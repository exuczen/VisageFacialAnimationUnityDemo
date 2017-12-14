using UnityEngine;
using System.Collections;

namespace DC
{
    public class OneInstance<Instance> : MonoBehaviour where Instance : OneInstance<Instance>
    {
        public static Instance instance;
        public bool isPersistant;

        public virtual void Awake ()
        {

            if (isPersistant)
            {
                if (!instance)
                {
                    instance = this as Instance;
                }
                else
                {
                    DestroyObject (gameObject);
                }
                DontDestroyOnLoad (gameObject);
            }
            else
            {
                instance = this as Instance;
            }
        }
    }
}