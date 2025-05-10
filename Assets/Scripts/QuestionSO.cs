using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Quiz Question", fileName = "New Question")]
public class QuestionSO : ScriptableObject
{
    [TextArea(3,10)]
    [SerializeField] string question = "Kitty";
    [SerializeField] string[] answers = new string[2];
    [SerializeField] int correctAnswerIndex ;
    

    public int GetCorrectAnswerIndex()
    {
        return Mathf.Clamp(correctAnswerIndex, 0, answers.Length - 1);
    }
    public string GetQuestion()
    {
        return question;
    }
    public string GetAnswer(int index)
    {
        return answers[index];
    }

    
}
