using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

public class BossFightTrigger : MonoBehaviour
{
    [SerializeField] private GameObject boss;
    [SerializeField] private GameObject bossFog;
    [SerializeField] private Transform bossFightTransform;
    [SerializeField] private PlayableDirector timeline;
    [SerializeField] private GameObject cutscenePause;
    [SerializeField] private GameObject canvas;
    
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            timeline.gameObject.SetActive(true);
            timeline.Play();
            canvas.SetActive(false);
            
            PlayerMovement playerMovement = other.GetComponent<PlayerMovement>();
            
            StartCoroutine(BossFight(playerMovement));
        }
    }

    private IEnumerator BossFight(PlayerMovement playerMovement)
    {
        playerMovement.MoveTo(bossFightTransform.position);
        yield return new WaitForSeconds(13f);
        cutscenePause.SetActive(true);
        playerMovement.StopAutoMove();
        timeline.gameObject.SetActive(false);
        timeline.Stop();
        boss.SetActive(true);
        bossFog.SetActive(true);
        canvas.SetActive(true);
        yield return new WaitForSeconds(1.5f);
        
        cutscenePause.SetActive(false);
        
    }
}
