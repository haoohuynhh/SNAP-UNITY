using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class Quiz : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI questionText;
    [SerializeField] List<QuestionSO> questions = new List<QuestionSO>();
    [SerializeField] GameObject[] answerButtons;
    [SerializeField] float switchInterval = 0.5f; // thời gian đổi lựa chọn
    [SerializeField] QuestionSO currentQuestion;

    private int currentSelectedIndex = 0;
    private float timer;

    public int CurrentSelectedIndex => currentSelectedIndex;
    public QuestionSO CurrentQuestion => currentQuestion;

    void Start()
    {
        DisplayQuestion();
    }

    void Update()
    {
        AutoSelectAnswer();
    }

    void DisplayQuestion()
    {
        currentQuestion = questions[Random.Range(0, questions.Count)];
        questionText.text = currentQuestion.GetQuestion();
        for (int i = 0; i < answerButtons.Length; i++)
        {
            answerButtons[i].GetComponentInChildren<TextMeshProUGUI>().text = currentQuestion.GetAnswer(i);
        }

        currentSelectedIndex = 0; // khởi đầu từ index 0
        HighlightButton(currentSelectedIndex);
    }

    void AutoSelectAnswer()
    {
        timer += Time.deltaTime;
        if (timer >= switchInterval)
        {
            timer = 0f;
            currentSelectedIndex = (currentSelectedIndex + 1) % answerButtons.Length;
            HighlightButton(currentSelectedIndex);
        }
    }

   public  void HighlightButton(int index)
    {
            Color highlightColor = new Color(0f, 1f, 239f/255f); // 00FFEF

        for (int i = 0; i < answerButtons.Length; i++)
        {
            var bg = answerButtons[i].GetComponent<UnityEngine.UI.Image>();
            if (bg != null)
            {
                bg.color = (i == index) ? highlightColor : Color.white;
            }
        }
    }
}
