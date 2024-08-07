using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class LifeSupport : Serviceable
{
    [SerializeField] private MeshRenderer[] renderers;
    private List<int> sequence = new List<int>();
    private int pointer = 0;
    private int rounds = 0;
    private const int roundsToPass = 3;
    private const int difficultyIncrease = 2;

    private Color defaultColor = Color.white;

    bool allowInput = false;

    [SerializeField] private UnityEvent OnBadSequence;

    private void Start()
    {
        if(renderers.Length > 0)
            defaultColor = renderers[0].material.color;
    }

    [ContextMenu("StartGame")]
    public void StartMinigame()
    {
        ClearMinigame();
        NewRound();
    }
  
    [ContextMenu("AddPoint")]
    public void AddPoint()
    {
        rounds++;
        if (rounds <= roundsToPass)
        {
            NewRound();
        }
        else
        {
            StartCoroutine(WinSequence());
            Fix();
        }
    }

    private void NewRound()
    {
        sequence.Clear();
        pointer = 0;

        for (int i = 0;i<(rounds*difficultyIncrease)+2;i++)
        {
            sequence.Add(Random.Range(0,renderers.Length));
        }
        StartCoroutine(NewRoundSequence());
    }

    private IEnumerator NewRoundSequence()
    {
        allowInput = false;
        yield return new WaitForSeconds(0.5f);
        for (int i=0;i<sequence.Count;i++)
        {
            renderers[sequence[i]].material.SetColor("_BaseColor",Color.blue);
            yield return new WaitForSeconds(1);
            renderers[sequence[i]].material.SetColor("_BaseColor", defaultColor);
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        allowInput = true;
    }

    private IEnumerator BadInputSequence()
    {
        allowInput = false;
        yield return new WaitForSeconds(0.5f);
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[sequence[i]].material.SetColor("_BaseColor", Color.red);
            }
            yield return new WaitForSeconds(0.5f);
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[sequence[i]].material.SetColor("_BaseColor", defaultColor);
            }
            yield return new WaitForSeconds(0.5f);
        }
        yield return new WaitForSeconds(0.5f);
        allowInput = true;
        OnBadSequence.Invoke();
    }

    private IEnumerator WinSequence()
    {
        allowInput = false;
        for (int j = 0; j < 4; j++)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_BaseColor", Color.green);
            }

            yield return new WaitForSeconds(0.5f);

            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.SetColor("_BaseColor", defaultColor);
            }

            yield return new WaitForSeconds(0.5f);
        }

        allowInput = true;
    }

    public void ClearMinigame()
    {
        sequence.Clear();
        pointer = 0;
        rounds = 0; 
    }

    public void InputData(int i)
    {
        if(allowInput==false) return;
        //Wprowadzono poprawny przycisk
        if (sequence[pointer] == i)
        {
            pointer++;
            //Wprowadzono poprawn¹ sekwencjê
            if(pointer >= sequence.Count)
            {
                AddPoint();
            }
        }
        else
        {
            ClearMinigame();
            StartCoroutine(BadInputSequence());
        }
    }
}
