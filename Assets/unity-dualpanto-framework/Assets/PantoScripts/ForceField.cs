using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DualPantoFramework
{
    abstract public class ForceField : MonoBehaviour
    {
        public bool onUpper = true;
        public bool onLower = true;
        protected abstract Vector3 GetCurrentForce(Collider other);
        protected abstract float GetCurrentStrength();

        void OnTriggerStay(Collider other)
        {
            // TODO: change here so that tags are correct and positions are calculated accordingly
            if (other.tag == "Element" && onUpper)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().ApplyForce(GetCurrentForce(other), GetCurrentStrength());
                Debug.Log("Forcefield!");
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(), Color.red);
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().ApplyForce(GetCurrentForce(other), GetCurrentStrength());
                Debug.Log("Forcefield!");
                Debug.DrawLine(other.transform.position, other.transform.position + GetCurrentForce(other) * GetCurrentStrength(), Color.red);
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Element" && onUpper)
            {
                GameObject.Find("Panto").GetComponent<UpperHandle>().StopApplyingForce();
                Debug.Log("Stop Forcefield!");
            }
            else if (other.tag == "ItHandle" && onLower)
            {
                GameObject.Find("Panto").GetComponent<LowerHandle>().StopApplyingForce();
            }
        }
    }
}