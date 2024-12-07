using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Netcode;
using UnityEngine;

public class TicTacToe : NetworkBehaviour
{
    [SerializeField] private TextMeshProUGUI[] outputs;
    [SerializeField] private GameObject resultPanel;
    [SerializeField] private TextMeshProUGUI resultOutput;
    private NetworkVariable<int> state = new NetworkVariable<int>(0);
    private int moves = 0;
    public int moveID=0;

    [ContextMenu("Play")]
    public void DebugInputState()
    {
        InputState(moveID);
    }
    public void InputState(int id)
    {
        if(CheckCell(id)!=0) return;
        ManageGameServerRpc(id);
    }

    [ServerRpc]
    private void ManageGameServerRpc(int id)
    {
        moves++;
        //O==1, X==2
        state.Value += ((moves % 2 == 1) ? 1 : 2)*(int)Mathf.Pow(10,id);

        RefreshUIClientRpc();

        int result = CheckWinConditions();

        if (result != 0) 
        {
            EndGameClientRpc(result);
            moves = 0;
            state.Value = 0;
            return;
        }

        if (result == 0 && moves == 9) 
        {
            EndGameClientRpc(3);
            moves = 0;
            state.Value = 0;
            return;
        }
       
    }

    private int CheckCell(int cell)
    {
        return (state.Value / (int)Mathf.Pow(10, cell))%10;
    }

    private int CheckWinConditions()
    {
        if (!IsServer) return 0;

        List<int> cells = new List<int>();
        for (int i = 0; i < 9; i++) 
        {
            cells.Add(CheckCell(i));
        }

        //rows
        for (int i = 0; i < 3; i++)
        {
            if (cells[3*i] == cells[3 * i + 1] && cells[3 * i + 1] == cells[3 * i + 2])
            {
                return cells[3 * i];
            }
        }

        //collumns
        for (int i = 0; i < 3; i++)
        {
            if (cells[0+i] == cells[3 + i] && cells[3 + i] == cells[6 + i])
            {
                return cells[0 + i];
            }
        }

        //diagonals
        if (cells[0] == cells[4] && cells[4] == cells[8]) return cells[0];
        if (cells[2] == cells[4] && cells[4] == cells[6]) return cells[2];

        return 0;
    }

    [ClientRpc]
    private void RefreshUIClientRpc()
    {
        for (int i = 0; i < 9; i++)
        {
            if(i>=outputs.Length)return;

            int d = CheckCell(i);
            if(d == 0) continue;
            outputs[i].SetText(d==1?"O":"X");
        }
    }

    [ClientRpc]
    private void EndGameClientRpc(int result)
    {

        StartCoroutine(WinSequence(result));
        for (int i = 0; i < outputs.Length; i++)
        {
            outputs[i].SetText("");
        }
    }

    private IEnumerator WinSequence(int result)
    {
        if (resultOutput && resultPanel) 
        {
            resultPanel.SetActive(true);
            switch (result)
            {
                case 1:
                    resultOutput.SetText("O wins");
                    Debug.Log("O wins");
                    break;
                case 2:
                    resultOutput.SetText("X wins");
                    Debug.Log("X wins");
                    break;
                case 3:
                    resultOutput.SetText("Match");
                    Debug.Log("Match");
                    break;
                default:
                    resultOutput.SetText("");
                    Debug.Log("Something went very wrong");
                    break;
            }
        }

        yield return new WaitForSeconds(3);
        resultOutput.SetText("");
        resultPanel.SetActive(false);

    }
}
