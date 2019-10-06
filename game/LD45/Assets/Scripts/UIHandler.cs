using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UIHandler : MonoBehaviour
{
    [SerializeField]
    Text Description;

    [SerializeField]
    GameObject confirmModal;

    [SerializeField]
    GameObject okModal;

    [SerializeField]
    Text confirmText;

    [SerializeField]
    Text okText;

    [SerializeField]
    AudioClip[] plops;

    [SerializeField]
    AudioClip[] clicks;

    [SerializeField]
    string levelinfo;

    [SerializeField]
    public string[] levelinfos;

    AudioSource audioSource;

    float opTimer = -1;

    public int infoidx = 0;

    enum Operation
    {
        RESTART,
        QUIT
    }

    enum OkOperation
    {
        WIN,
        NO_EFFECT,
        FINISH
    }

    Operation operation;
    OkOperation okOperation = OkOperation.NO_EFFECT;

    // Start is called before the first frame update
    void Start()
    {
        if (levelinfo.Length > 0)
        {
            levelinfos = new string[1];
            levelinfos[0] = levelinfo;
        }
        audioSource = GetComponent<AudioSource>();
        if (levelinfos != null && levelinfos.Length > 0)
        {
            ShowInfo(levelinfos[infoidx++]);
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (opTimer > 0 && opTimer < Time.time)
        {
            doOperation();
            opTimer = -1;
        }
    }

    public void UpdateDescription(string description)
    {
        Description.text = description.Replace("\\n", "\n");
    }

    public void Quit()
    {
        audioSource.clip = Util.getRandom(plops);
        audioSource.Play();
        confirmModal.SetActive(true);
        
        confirmText.text = "Quit game.\n\nAre you sure?";
        operation = Operation.QUIT;
    }

    public void Restart()
    {
        audioSource.clip = Util.getRandom(plops);
        audioSource.Play();
        confirmModal.SetActive(true);
        confirmText.text = "Restart current world.\n\nAre you sure?";
        operation = Operation.RESTART;
    }

    public void ConfirmModal()
    {
        audioSource.clip = Util.getRandom(clicks);
        audioSource.Play();
        confirmModal.SetActive(false);
        opTimer = Time.time + 0.2f;
    }

    public void CancelModal()
    {
        audioSource.clip = Util.getRandom(clicks);
        audioSource.Play();
        confirmModal.SetActive(false);
    }

    void doOperation()
    {
        if (operation == Operation.RESTART)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }
        else if (operation == Operation.QUIT)
        {
            Application.Quit();
        }
    }

    public void ShowInfo(string info)
    {
        if (info.Length > 0 && okOperation == OkOperation.NO_EFFECT)
        {
            okText.text = info.Replace("\\n", "\n");
            okModal.SetActive(true);
        }
    }

    public void Victory()
    {
        int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
        if (SceneManager.sceneCount > nextSceneIndex)
        {
            okText.text = "Victory!";
            okOperation = OkOperation.WIN;
        }
        else
        {
            okText.text = "Thanks for playing!";
            okOperation = OkOperation.FINISH;
        }
        okModal.SetActive(true);
    }

    public void OK()
    {
        audioSource.clip = Util.getRandom(clicks);
        audioSource.Play();

        if (okOperation == OkOperation.NO_EFFECT)
        {
            if (infoidx < levelinfos.Length)
            {
                ShowInfo(levelinfos[infoidx++]);
            }
            else
            {
                okModal.SetActive(false);
            }
        }
        else if (okOperation == OkOperation.WIN)
        {
            int nextSceneIndex = SceneManager.GetActiveScene().buildIndex + 1;
            SceneManager.LoadScene(nextSceneIndex);
            okModal.SetActive(false);
        }
        else if (okOperation == OkOperation.FINISH)
        {
            Application.Quit();
            return;
        }
    }
}
