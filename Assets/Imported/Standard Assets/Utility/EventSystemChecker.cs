using UnityEngine;
using UnityEngine.EventSystems;

namespace Assets.Imported.Standard_Assets.Utility
{
    public class EventSystemChecker : MonoBehaviour
    {
        //public GameObject eventSystem;

        // Use this for initialization
        void Awake ()
        {
            if(!FindObjectOfType<EventSystem>())
            {
                //Instantiate(eventSystem);
                GameObject obj = new GameObject("EventSystem");
                obj.AddComponent<EventSystem>();
                obj.AddComponent<StandaloneInputModule>().forceModuleActive = true;
            }
        }
    }
}
