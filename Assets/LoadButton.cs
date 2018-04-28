using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using SFB;

[RequireComponent(typeof(Button))]
public class LoadButton : MonoBehaviour, IPointerDownHandler
{
    public string Title = "";
    public string FileName = "";
    public string Directory = "";
    public string Extension = "";
    public bool Multiselect = false;

    public JigsawGenerator jigsawGenerator;

#if UNITY_WEBGL && !UNITY_EDITOR
    //
    // WebGL
    //
    [DllImport("__Internal")]
    private static extern void UploadFile(string id);

    public void OnPointerDown(PointerEventData eventData) {
        UploadFile(gameObject.name);
    }

    // Called from browser
    public void OnFileUploaded(string url) {
        StartCoroutine(OutputRoutine(url));
    }
#else
    //
    // Standalone platforms & editor
    //
    public void OnPointerDown(PointerEventData eventData) { }

    void Start()
    {
        var button = GetComponent<Button>();
        button.onClick.AddListener(OnClick);
    }

    private void OnClick()
    {
        var paths = StandaloneFileBrowser.OpenFilePanel(Title, Directory, Extension, Multiselect);
        if (paths.Length > 0)
        {
            StartCoroutine(OutputRoutine(new System.Uri(paths[0]).AbsoluteUri));
        }
    }
#endif

    private IEnumerator OutputRoutine(string url)
    {
        Debug.Log("URL: " + url);
        var loader = new WWW(url);
        yield return loader;
        var nxObj = transform.parent.Find("Nx");
        var nxStr = nxObj.Find("Text").GetComponentInChildren<Text>().text;
        
        var nx = int.Parse(nxStr);

        var nyObj = transform.parent.Find("Ny");
        var nyStr = nyObj.Find("Text").GetComponentInChildren<Text>().text;
        var ny = int.Parse(nyStr);

        jigsawGenerator.Generate(nx, ny, loader.texture);
        gameObject.SetActive(false);
        nxObj.gameObject.SetActive(false);
        nyObj.gameObject.SetActive(false);
    }
}