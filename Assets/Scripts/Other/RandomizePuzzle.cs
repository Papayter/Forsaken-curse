using UnityEngine;
using Random = UnityEngine.Random;

namespace Other
{
    public class RandomizePuzzle : MonoBehaviour
    {
        [SerializeField] private GameObject[] platformDown;
        [SerializeField] private GameObject[] platformTop;
        
        [SerializeField] private GameObject[] puzzleDown;
        [SerializeField] private GameObject[] puzzleTop;

        private void Start()
        {
            var randomDownIndex = Random.Range(0, puzzleDown.Length);
            var randomTopIndex = Random.Range(0, puzzleTop.Length);
            
            platformDown[randomDownIndex].tag = "Untagged";
            platformTop[randomTopIndex].tag = "Untagged";
            
            puzzleDown[randomDownIndex].GetComponent<Renderer>().material.color = Color.green;
            puzzleTop[randomTopIndex].GetComponent<Renderer>().material.color = Color.green;
            
        }
    }
}