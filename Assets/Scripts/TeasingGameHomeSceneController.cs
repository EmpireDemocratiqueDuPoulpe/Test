using UnityEngine;
using SceneTransitionSystem;
using UnityEngine.SceneManagement;

namespace TeasingGame
{
    public enum TeasingGameScene :int 
    {
        Home,
        Game,
    }
    
    public class TeasingGameHomeSceneController : MonoBehaviour
    {
        public TeasingGameScene SceneForButton;

        public void GoToGameScene()
        {
            STSSceneManager.LoadScene(SceneForButton.ToString());
        }
        
        public void ReloadScene()
        {
            // The scene doesn't reload when using this method
            //STSSceneManager.LoadScene(STSSceneManager.GetActiveScene().buildIndex);
            
            SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
        }
    }
}