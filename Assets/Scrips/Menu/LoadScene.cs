using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LoadScene : MonoBehaviour
{
  public void LoadByIndex()
  {
    UnityEngine.SceneManagement.SceneManager.LoadScene("Lv1");
    }
}
