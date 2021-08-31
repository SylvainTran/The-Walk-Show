using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Video;

public class VideoPlayerURL : MonoBehaviour
{
    public string videoUrl;

    void Start()
    {
        VideoPlayer vp = GetComponent<VideoPlayer>();
        try
        {
            if(vp.url == null || vp.url.Length == 0)
            {
                vp.url = System.IO.Path.Combine(Application.streamingAssetsPath, videoUrl);
            }
            if(!vp.isPlaying)
            {
                vp.Play();
            }
        } catch(Exception e)
        {
            Debug.LogError(e.Message);
        }
    }
}
