using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayAction : MonoBehaviour {

	// Use this for initialization
	public void ClickAction () 
    {
        GameMaster.level = 1;
        SceneManager.LoadScene("maze");
    }
}
