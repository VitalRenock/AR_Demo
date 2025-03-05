using Assets.Scripts;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "QuestionsSet", menuName = "Scriptable Objects/QuestionsSet")]
public class QuestionsSet : ScriptableObject
{
    public List<Question> QuestionsList = new List<Question>();
}
