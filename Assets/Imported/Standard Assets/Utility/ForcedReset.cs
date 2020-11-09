using Assets.Imported.Standard_Assets.CrossPlatformInput.Scripts;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable 618
namespace Assets.Imported.Standard_Assets.Utility
{
    [RequireComponent(typeof (Image))]
    public class ForcedReset : MonoBehaviour
    {
        private void Update()
        {
            // if we have forced a reset ...
            if (CrossPlatformInputManager.GetButtonDown("ResetObject"))
            {
                //... reload the scene
                SceneManager.LoadScene(SceneManager.GetSceneAt(0).name);
            }
        }
    }
}
