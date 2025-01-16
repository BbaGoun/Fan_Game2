using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ActionPart
{
    public class Map_Interaction : MonoBehaviour
    {
        [SerializeField]
        List<GameObject> locals = new List<GameObject>();

        public void SelectLocal(int index)
        {
            foreach (GameObject go in locals)
            {
                go.SetActive(false);
            }

            locals[index].SetActive(true);
        }
    }
}
