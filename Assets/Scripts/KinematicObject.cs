using Unity.VisualScripting;
using UnityEditor.Rendering;
using UnityEngine;
using UnityEngine.Timeline;

namespace ActionPart
{
    /// <summary>
    /// Implements game physics for some in game entity.
    /// </summary>
    public class KinematicObject : MonoBehaviour
    {
        /// <summary>
        /// The current velocity of the entity.
        /// </summary>
        [SerializeField]
        public Vector2 velocity;
        private float speedMultiplier;

        /// <summary>
        /// Is the entity currently sitting on a surface?
        /// </summary>

        [SerializeField]
        public bool isGrounded = true;
        public bool isHeading { get; private set; }

        private BoxCollider2D boxCollider;
        private CapsuleCollider2D capsuleCollider;
        private Vector2 colliderSize;
        private Vector2 colliderOffset;
        private Rigidbody2D body;
        private float direction;
        private RaycastHit2D[] hitBuffer = new RaycastHit2D[16];
        [SerializeField]
        private ContactFilter2D contactFilter;

        private const float minMoveDistance = 0.001f;
        private float shellRadius = 0.01f;

        [Header("Ground/Slope Parameter")]
        [SerializeField]
        protected float coyoteTime;
        private float groundLastTime;
        [SerializeField]
        protected Vector2 _groundCheckBoxSize;
        protected Vector2 groundCheckBoxSize;
        [SerializeField]
        protected Vector2 _headingCheckBoxSize;
        protected Vector2 headingCheckBoxSize;
        [SerializeField, Range(0f, 0.999f)]
        protected float maxUnitSlopeY = 0.6f; // 경사로 단위벡터의 최대 Y
        
        private float minUnitSlopeY = 0.01f;

        [SerializeField]
        protected float _horizontalSlopeCheckDistance;
        protected float horizontalSlopeCheckDistance;
        [SerializeField]
        protected float _verticalSlopeCheckDistance;
        protected float verticalSlopeCheckDistance;
        [SerializeField, Min(1f)]
        protected float checkDistanceFactor = 1f;

        [SerializeField]
        protected Vector2 _frontSlopeCheckOffset;
        protected Vector2 frontSlopeCheckOffset;
        [SerializeField]
        protected Vector2 _downSlopeCheckOffset;
        protected Vector2 downSlopeCheckOffset;
        [SerializeField]
        protected Vector2 _backSlopeCheckOffset;
        protected Vector2 backSlopeCheckOffset;
        
        private Vector2 frontSlopeVec;
        private Vector2 downSlopeVec;
        private Vector2 backSlopeVec;

        [SerializeField]
        private bool isOnFrontSlope;
        [SerializeField]
        private bool isOnDownSlope;
        [SerializeField]
        private bool isOnBackSlope;

        [SerializeField]
        private Vector2 slopeVec;
        [SerializeField]
        private Vector2 contactNormal;
        private Vector2 hitPoint;

        private Vector2 finalMove;

        [SerializeField]
        protected bool canWalkOnSlope;

        // 바닥으로부터 떨어져있을 거리
        public float _groundOffsetY;
        private float groundOffsetY;
        private float highestGroundY;

        /// <summary>
        /// Teleport to some position.
        /// </summary>
        /// <param name="position"></param>
        public void Teleport(Vector3 position)
        {
            body.position = position;
            velocity *= 0;
            body.velocity *= 0;
        }

        protected virtual void OnEnable()
        {
            body = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            capsuleCollider = GetComponent<CapsuleCollider2D>();
            body.isKinematic = true;

            velocity = Vector2.zero;
        }

        protected virtual void Start()
        {
            speedMultiplier = 1f;
            contactFilter.useTriggers = false;
            contactFilter.useLayerMask = true;
            contactFilter.SetLayerMask(LayerMask.GetMask("Ground"));

            boxCollider.enabled = true;
            capsuleCollider.enabled = false;
        }

        public void SetSpeedMultiplier(float _speedMultiplier)
        {
            speedMultiplier = _speedMultiplier;
        }

        protected virtual void ComputeVelocity()
        {
            // 상속 클래스에서 구현
        }

        protected virtual void FixedUpdate()
        {
            colliderSize = capsuleCollider.size * Mathf.Abs(transform.localScale.x);
            colliderOffset = capsuleCollider.offset * Mathf.Abs(transform.localScale.x);

            groundCheckBoxSize = _groundCheckBoxSize * Mathf.Abs(transform.localScale.x);
            headingCheckBoxSize = _headingCheckBoxSize * Mathf.Abs(transform.localScale.x);
            
            horizontalSlopeCheckDistance = _horizontalSlopeCheckDistance * Mathf.Abs(transform.localScale.x);
            verticalSlopeCheckDistance = _verticalSlopeCheckDistance * Mathf.Abs(transform.localScale.x);
            
            frontSlopeCheckOffset = _frontSlopeCheckOffset * Mathf.Abs(transform.localScale.x);
            downSlopeCheckOffset = _downSlopeCheckOffset * Mathf.Abs(transform.localScale.x);
            backSlopeCheckOffset = _backSlopeCheckOffset * Mathf.Abs(transform.localScale.x);

            groundOffsetY = _groundOffsetY * Mathf.Abs(transform.localScale.x);

            ComputeVelocity();

            ApplyMovement();
        }

        void ApplyMovement()
        {
            Vector2 deltaPosition;
            Vector2 move;
            slopeVec = Vector2.zero;

            GroundCheck();
            HeadingCheck();

            if (isGrounded && velocity.y <= 0f)
            {
                AdjustVelocity();
                deltaPosition = velocity * speedMultiplier * Time.deltaTime;

                CheckSlope();

                if (!isOnFrontSlope && !isOnDownSlope && !isOnBackSlope)
                {
                    boxCollider.enabled = true;
                    capsuleCollider.enabled = false;

                    // 가만히 있는데 낑기는지 확인
                    /*var countX = body.Cast(Vector2.up, contactFilter, hitBuffer, shellRadius);
                    if (countX > 0)
                    {
                        Debug.Log("낑기니까 바꿔줄게");
                        boxCollider.enabled = false;
                        capsuleCollider.enabled = true;
                    }*/

                    move = deltaPosition;
                }
                else
                {
                    boxCollider.enabled = false;
                    capsuleCollider.enabled = true;
                    if(Mathf.Abs(deltaPosition.x) < minMoveDistance)
                    {
                        move = deltaPosition;
                    }
                    else
                    {
                        move = slopeVec * deltaPosition.x;
                    }
                }
            }
            else
            {
                isOnFrontSlope = false;
                isOnDownSlope = false;
                isOnBackSlope = false;

                deltaPosition = velocity * speedMultiplier * Time.deltaTime;
                move = deltaPosition;
            }

            PerformMovement(move);
        }


        /// <summary>
        /// 지면 착지 검사
        /// </summary>
        /*private void GroundCheck()
        {
            if (velocity.y > 0f)
            {
                isGrounded = false;
            }
            else if (velocity.y <= 0)
            {
                direction = transform.localScale.x;
                Vector2 checkPos = transform.position - new Vector3(direction * -colliderOffset.x, colliderSize.y / 2 - colliderOffset.y);
                int count = 10;
                int hitCount = 0;
                for (int i = 0; i <= count; i++)
                {
                    var deltaX = groundCheckBoxSize.x * 0.1f * (i - 5);
                    var checkY = groundCheckBoxSize.y;
                    if (isOnFrontSlope || isOnDownSlope || isOnBackSlope)
                        checkY *= 5f;
                    var deltaY = checkY * 0.5f;

                    var originX = checkPos.x + deltaX;
                    var originY = checkPos.y + deltaY;
                    Vector2 origin = new Vector2(originX, originY);
                    var hit = Physics2D.Raycast(origin, Vector2.down, checkY, contactFilter.layerMask);
                    Debug.DrawRay(origin, Vector2.down * checkY, Color.green);

                    if (hit)
                    {
                        Debug.DrawRay(hit.point, hit.normal.normalized * 5, Color.red);
                        var perpendicular = -Vector2.Perpendicular(hit.normal.normalized);
                        if (Mathf.Abs(perpendicular.y) < 1f)
                        {
                            if (hitCount == 0)
                                highestGroundY = hit.point.y;
                            else
                            {
                                if (highestGroundY < hit.point.y)
                                {
                                    highestGroundY = hit.point.y;
                                    //Debug.Log("높이 갱신 : " + highestGroundY);
                                }
                            }
                            hitCount++;
                            isGrounded = true;
                            groundLastTime = Time.time;
                        }
                    }
                }

                if (hitCount == 0)
                {
                    if (isGrounded && velocity.y < 0f && Time.time - groundLastTime < coyoteTime)
                        isGrounded = true;
                    else
                        isGrounded = false;
                }
            }
        }*/

        /// <summary>
        ///  지면감지 바꿈, 일단 잘 작동함.
        /// </summary>
        private void GroundCheck()
        {
            if(velocity.y > 0f)
            {
                isGrounded = false;
                return;
            }
            var count = body.Cast(Vector2.down, contactFilter, hitBuffer, shellRadius);
            if (count > 0)
            {
                for(int i = 0;i<count;i++)
                {
                    Vector2 normal = hitBuffer[i].normal;
                    if(Mathf.Abs(normal.y) >= 1 - maxUnitSlopeY)
                    {
                        isGrounded = true;
                        contactNormal = normal;
                        hitPoint = hitBuffer[i].point;
                        Debug.DrawRay(hitBuffer[i].point, normal * 10, Color.magenta);
                    }
                }
            }
            else 
            { 
                isGrounded = false;
                contactNormal = Vector2.up;
            }
        }

        /*private void HeadingCheck()
        {
            direction = transform.localScale.x;
            Vector2 checkPos = transform.position + new Vector3(direction * colliderOffset.x, colliderSize.y / 2 + colliderOffset.y);
            Collider2D hit = Physics2D.OverlapBox(checkPos, headingCheckBoxSize, 0f, contactFilter.layerMask);

            if (hit)
            {
                isHeading = true;

                Utility.DrawBox(checkPos, headingCheckBoxSize, Color.red);
            }
            else
            {
                isHeading = false;
            }
        }*/

        /// <summary>
        ///  헤딩감지 바꿈, 일단 잘 작동함.
        /// </summary>
        private void HeadingCheck()
        {
            var count = body.Cast(Vector2.up, contactFilter, hitBuffer, shellRadius);
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    Vector2 normal = hitBuffer[i].normal;
                    if (Mathf.Abs(normal.y) >= 1)
                    {
                        isHeading = true;
                    }
                }
            }
            else
            {
                isHeading = false;
            }
        }

        private void AdjustVelocity()
        {
            Vector2 alongSlope = ProjectOnContactVector(Vector2.right).normalized;

            var tempVelocity = alongSlope * velocity.x;
            Debug.DrawRay(hitPoint, tempVelocity * 10, Color.red);
        }

        /// <summary>
        /// 닿은 지면을 따라 움직이는 벡터를 생성
        /// </summary>
        /// <param name="vector"></param>
        /// <returns></returns>
        private Vector2 ProjectOnContactVector(Vector2 vector)
        {
            return vector - contactNormal * Vector2.Dot(vector, contactNormal);
        }

        /// <summary>
        /// 경사로 체크
        /// </summary>
        private void CheckSlope()
        {
            /*CheckFrontSlope();

            if (!isOnFrontSlope)
                CheckBackSlope();
            else
            {
                isOnBackSlope = false;
                backSlopeVec = Vector2.zero;
            }

            if (!isOnFrontSlope && !isOnBackSlope)
                CheckDownSlope();
            else
            {
                isOnDownSlope = false;
                downSlopeVec = Vector2.zero;
            }*/

            CheckDownSlope();

            if(!isOnDownSlope)
                CheckFrontSlope();
            else
            {
                isOnFrontSlope = false;
                frontSlopeVec = Vector2.zero;
            }

            if(!isOnDownSlope && !isOnFrontSlope)
                CheckBackSlope();
            else
            {
                isOnBackSlope = false;
                backSlopeVec = Vector2.zero;
            }

            ApplySlope();
        }

        private void CheckFrontSlope()
        {
            direction = transform.localScale.x;

            Vector2 frontCheckPos = transform.position - new Vector3(direction * (-frontSlopeCheckOffset.x - colliderOffset.x), colliderSize.y / 2 - frontSlopeCheckOffset.y - colliderOffset.y);
            RaycastHit2D frontHit = new RaycastHit2D();
            if (isOnFrontSlope)
                frontHit = Physics2D.Raycast(frontCheckPos, direction * Vector2.right, horizontalSlopeCheckDistance * checkDistanceFactor, contactFilter.layerMask);
            else
                frontHit = Physics2D.Raycast(frontCheckPos, direction * Vector2.right, horizontalSlopeCheckDistance, contactFilter.layerMask);


            if (frontHit)
            {
                frontSlopeVec = -Vector2.Perpendicular(frontHit.normal).normalized;
                if (Mathf.Abs(frontSlopeVec.y) > minUnitSlopeY && Mathf.Abs(frontSlopeVec.y) < maxUnitSlopeY)
                {
                    /*Debug.DrawRay(frontCheckPos, direction * Vector2.right * horizontalSlopeCheckDistance, Color.green);
                    if (isOnFrontSlope)
                        Debug.DrawRay(frontCheckPos, direction * Vector2.right * horizontalSlopeCheckDistance * checkDistanceFactor, Color.blue);
                    Debug.DrawRay(frontHit.point, frontSlopeVec, Color.cyan);
                    Debug.DrawRay(frontHit.point, frontHit.normal, Color.magenta);*/
                    isOnFrontSlope = true;
                    return;
                }
            }

            isOnFrontSlope = false;
            frontSlopeVec = Vector2.zero;
        }

        private void CheckDownSlope()
        {
            direction = transform.localScale.x;

            RaycastHit2D downHit = new RaycastHit2D();
            Vector2 downCheckPos = transform.position - new Vector3(direction * (-downSlopeCheckOffset.x - colliderOffset.x), colliderSize.y / 2 - downSlopeCheckOffset.y - colliderOffset.y);
            if (isOnDownSlope)
                downHit = Physics2D.Raycast(downCheckPos, Vector2.down, verticalSlopeCheckDistance * checkDistanceFactor, contactFilter.layerMask);
            else
                downHit = Physics2D.Raycast(downCheckPos, Vector2.down, verticalSlopeCheckDistance, contactFilter.layerMask);

            if (downHit)
            {
                downSlopeVec = -Vector2.Perpendicular(downHit.normal).normalized;
                if (Mathf.Abs(downSlopeVec.y) > minUnitSlopeY && Mathf.Abs(downSlopeVec.y) < maxUnitSlopeY)
                {
                    /*Debug.DrawRay(downCheckPos, Vector2.down * verticalSlopeCheckDistance, Color.yellow);
                    if (isOnDownSlope)
                        Debug.DrawRay(downCheckPos, Vector2.down * verticalSlopeCheckDistance * checkDistanceFactor, Color.blue);
                    Debug.DrawRay(downHit.point, downSlopeVec, Color.cyan);
                    Debug.DrawRay(downHit.point, downHit.normal, Color.magenta);*/
                    isOnDownSlope = true;
                    return;
                }
            }

            isOnDownSlope = false;
            downSlopeVec = Vector2.zero;
        }


        private void CheckBackSlope()
        {
            direction = transform.localScale.x;

            RaycastHit2D backHit = new RaycastHit2D();
            Vector2 backCheckPos = transform.position - new Vector3(direction * (-backSlopeCheckOffset.x - colliderOffset.x), colliderSize.y / 2 - backSlopeCheckOffset.y - colliderOffset.y);
            if (isOnBackSlope)
                backHit = Physics2D.Raycast(backCheckPos, direction * Vector2.left, horizontalSlopeCheckDistance * checkDistanceFactor, contactFilter.layerMask);
            else
                backHit = Physics2D.Raycast(backCheckPos, direction * Vector2.left, horizontalSlopeCheckDistance, contactFilter.layerMask);

            if (backHit)
            {
                backSlopeVec = -Vector2.Perpendicular(backHit.normal).normalized;
                if (Mathf.Abs(backSlopeVec.y) > minUnitSlopeY && Mathf.Abs(backSlopeVec.y) < maxUnitSlopeY)
                {
                    /*Debug.DrawRay(backCheckPos, direction * Vector2.left * horizontalSlopeCheckDistance, Color.red);
                    if(isOnBackSlope)
                        Debug.DrawRay(backCheckPos, direction * Vector2.left * horizontalSlopeCheckDistance * checkDistanceFactor, Color.blue);
                    Debug.DrawRay(backHit.point, backSlopeVec, Color.cyan);
                    Debug.DrawRay(backHit.point, backHit.normal, Color.magenta);*/
                    isOnBackSlope = true;
                    return;
                }
            }

            isOnBackSlope = false;
            backSlopeVec = Vector2.zero;
        }

        private void ApplySlope()
        {
            if (isOnFrontSlope)
            {
                slopeVec = frontSlopeVec;
            }
            else if (isOnBackSlope)
            {
                slopeVec = backSlopeVec;
            }
            else if (isOnDownSlope)
            {
                slopeVec = downSlopeVec;
            }

            if (slopeVec.y < maxUnitSlopeY)
            {
                canWalkOnSlope = true;
            }
            else
            {
                canWalkOnSlope = false;
            }
        }

        private void PerformMovement(Vector2 move)
        {
            //Debug.Log("before move: " + move);

            var isFloatingAir = highestGroundY + groundOffsetY < body.position.y;

            if (isGrounded && isFloatingAir && move.y > 0)
            {
                move.y = -move.y;
                //Debug.Log("이젠 내려가거라");
            }

            Vector2 moveX = Vector2.right * move.x;
            Vector2 moveY = Vector2.up * move.y;

            var distanceX = moveX.magnitude;
            var distanceY = moveY.magnitude;
            var distance = move.magnitude;

            Vector2 moveNormalVec = Vector2.zero;
            RaycastHit2D hitPoint = new RaycastHit2D();
            bool isHit = false;


            if (distanceX > minMoveDistance)
            {
                var countX = body.Cast(moveX, contactFilter, hitBuffer, distanceX + shellRadius);

                for (var i = 0; i < countX; i++)
                {
                    var modifiedDistanceX = hitBuffer[i].distance - shellRadius;
                    if(modifiedDistanceX < distanceX)
                    {
                        distanceX = modifiedDistanceX;
                    }
                }

            }
            if(distanceY > minMoveDistance)
            {
                var countY = body.Cast(moveY, contactFilter, hitBuffer, distanceY + shellRadius);

                for (var i = 0; i < countY; i++)
                {
                    var modifiedDistanceY = hitBuffer[i].distance - shellRadius;
                    if(modifiedDistanceY < distanceY)
                    {
                        distanceY = modifiedDistanceY;
                    }
                }
            }
            if (distance > minMoveDistance)
            {
                int count = 0;
                count = body.Cast(move, contactFilter, hitBuffer, distance + shellRadius);

                for (var i = 0; i < count; i++)
                {
                    var modifiedDistance = hitBuffer[i].distance - shellRadius;
                    if (modifiedDistance < distance)
                    {
                        distance = modifiedDistance;
                        moveNormalVec = -Vector2.Perpendicular(hitBuffer[i].normal).normalized;
                        hitPoint = hitBuffer[i];
                        isHit = true;
                    }
                }

                if(isHit)
                {
                    // move의 방향에서 만난 벽이 수직벽이 아닌 경우 (X방향과 Y 방향)
                    if (Mathf.Abs(moveNormalVec.y) < 1 - minUnitSlopeY && Mathf.Abs(moveNormalVec.y) > minUnitSlopeY)
                    {
                        var xOfDistance = Mathf.Abs((move.normalized * distance).x);
                        if (xOfDistance < distanceX)
                        {
                            distanceX = xOfDistance;
                        }
                        var yOfDistance = Mathf.Abs((move.normalized * distance).y);
                        if (yOfDistance < distanceY)
                        {
                            distanceY = yOfDistance;
                        }
                    }

                }
            }

            /*if (distanceX < 0)
                distanceX = 0;
            if (distanceY < 0)
                distanceY = 0;*/

            if(!isFloatingAir && isGrounded && (Mathf.Abs(moveY.y) > minMoveDistance && Mathf.Abs(moveX.x) < minMoveDistance) 
                    && (isOnFrontSlope || isOnDownSlope || isOnBackSlope))
            {
                //Debug.Log("내려가지말아다오");
                distanceY = 0f;
            }

            move = moveX.normalized * distanceX + moveY.normalized * distanceY;
            Debug.DrawRay(body.position, move * 100, Color.white);
            //Debug.Log("After move: " + move);

            var targetPosition = body.position + move;
            body.MovePosition(targetPosition);
            //body.position = body.position + move;
            finalMove = move;
        }
    }
}