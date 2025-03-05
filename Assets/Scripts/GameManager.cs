using Assets.Scripts;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    /* On Start :
     * 1) On charge tout les gameobjects AR sur la scène à partir du QuestionsSet dans une liste GO AR
     * 2) On lance ShowNextQuestion()
     * 
     * ShowNextQuestion() :
     * 3) On charge la prochaine question :
     *      On modifie le texte de la question affichée à partir du QuestionSet(currentRound)
     *      On modifie le texte des stats du joueur
     *      
     *      Pour tout les GO AR de la liste GO AR :
     *          On enlève tout ses listeners
     *          Si l'index du GO AR courant égal le currentRound :
     *              On ajoute un listener de WinRound()
     *          Sinon
     *              On ajoute un listener de LostRound()
     * 
     *      
     * WinRound() :
     *      Pour tout les GO AR de la liste GO AR :
     *          On enlève tout ses listeners
     *          
     *      On affiche un message de victoire
     *      (On joue un son)
     *      On ajoute un point de score au joueur
     *      On augmente de un le currentRound
     *      
     *      On attends x secondes (si possible afficher un décompte)
     *      
     *      On lance ShowNextQuestion()
     * 
     * LostRound() :
     *      (On enlève tout les listeners du GO AR qui a lancé l'event)
     * 
     *      On affiche un message de défaite
     *      (On joue un son)
     *      On enlève un point de vie au joueur
     *      
     *      Si le joueur n'a plus de points de vie :
     *          On lance la Scène de défaite
     *          
     *      On attends x secondes (si possible afficher un décompte)
     *      
     */

    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI StatsText;
    public GameObject VictoryPanel;
    public GameObject DefeatPanel;
    public float PanelTimer;

    public QuestionsSet QuestionsSet;
    public PlayerSet PlayerSet;

    private int currentQuestionIndex = 0;
    private Player currentPlayer;
    /// <summary>
    /// Liste des objets instanciés sur la scène
    /// </summary>
    [SerializeField] private List<GameObject> spawnedQuestions = new List<GameObject>();

    void Start()
    {
        try
        {
            // On charge les questions sur la scène
            LoadQuestions();

            // On désactive le panneau de Victoire et de Défaite
            VictoryPanel.SetActive(false);
            DefeatPanel.SetActive(false);

            // On copie les presets joueur sur le joueur courant
            currentPlayer = new Player();
            currentPlayer.SetLifePoint(PlayerSet.LifePoint);
            currentPlayer.SetScorePoint(PlayerSet.ScorePoint);

            // On affiche la première question
            ShowNextQuestion();
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void LoadQuestions()
    {
        try
        {
            // Nettoyer la scène en supprimant les questions précédentes (si besoin)
            foreach (var question in spawnedQuestions)
            {
                Destroy(question);
            }
            spawnedQuestions.Clear();

            // Instancier chaque GameObject de la liste du SO QuestionsSet
            foreach (Question question in QuestionsSet.QuestionsList)
            {
                GameObject newQuestion = Instantiate(question.ImageTarget, Vector3.zero, Quaternion.identity);
                spawnedQuestions.Add(newQuestion);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    void ShowNextQuestion()
    {
        try
        {
            // Si il y a encore des questions dans le QuestionsSet à l'index courant :
            if (currentQuestionIndex < QuestionsSet.QuestionsList.Count)
            {
                // On mets à jour le texte des composants Question et Stats
                QuestionText.text = QuestionsSet.QuestionsList[currentQuestionIndex].Text;
                StatsText.text = GetStatsText(currentPlayer, currentQuestionIndex);

                // Pour chaque GO AR sur la scène :
                foreach (GameObject gameObjectAR in spawnedQuestions)
                {
                    // On récupère le composant DefaultObserverEventHandler que Vuforia utilise
                    DefaultObserverEventHandler eventHandler = gameObjectAR.GetComponent<DefaultObserverEventHandler>();

                    // On enlève tout les listeners sur OnTargetFound()
                    eventHandler.OnTargetFound.RemoveAllListeners();

                    // Si l'index dans la liste du GO AR correspond à l'index de la question, on ajoute WinRound() sinon LostRound()
                    if (spawnedQuestions.IndexOf(gameObjectAR) == currentQuestionIndex)
                        eventHandler.OnTargetFound.AddListener(WinRound);
                    else
                        eventHandler.OnTargetFound.AddListener(() => LostRound(eventHandler));
                }
            }
            else
            {
                // Il n'y a plus de question => Fin de la partie
                SceneManager.LoadScene("Menu");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void WinRound()
    {
        try
        {
            if (spawnedQuestions != null && spawnedQuestions.Count > 0)
            {
                // Pour chaque GO AR sur la scène, on enlève les listeners sur OnTargetFound() :
                foreach (GameObject gameObjectAR in spawnedQuestions)
                    gameObjectAR.GetComponent<DefaultObserverEventHandler>().OnTargetFound.RemoveAllListeners();
            }

            // On ajoute un point de score au joueur
            currentPlayer.IncreaseScorePoint();

            // On mets à jour les stats du joueur sur l'écran
            StatsText.text = GetStatsText(currentPlayer, currentQuestionIndex);

            // (On joue un son)

            // On affiche un message de victoire
            StartCoroutine(ShowVictoryPanel());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    public void LostRound(DefaultObserverEventHandler eventHandler)
    {
        try
        {
            // On enlève tout les listeners du GO AR qui a lancé l'event
            eventHandler.OnTargetFound.RemoveAllListeners();

            // On enlève un point de vie au joueur
            currentPlayer.DecreaseLifePoint();

            // On mets à jour les stats du joueur sur l'écran
            StatsText.text = GetStatsText(currentPlayer, currentQuestionIndex);

            // Si le joueur n'a plus de points de vie :
            if (currentPlayer.LifePoint <= 0)
            {
                // Il n'y a plus de points de vie => Fin de la partie
                SceneManager.LoadScene("Menu");
            }

            // (On joue un son)

            // On affiche un message de défaite
            StartCoroutine(ShowDefeatPanel());
        }
        catch (Exception ex)
        {
            Console.WriteLine(ex.Message);
        }
    }

    IEnumerator ShowVictoryPanel()
    {
        // On affiche le panel de victoire
        VictoryPanel.SetActive(true);

        // On attendq 3 secondes
        yield return new WaitForSeconds(PanelTimer);

        // On masque le panel
        VictoryPanel.SetActive(false);

        // On augmente l'index pour la question suivante
        currentQuestionIndex++;

        // On passe à la question suivante
        ShowNextQuestion();
    }

    IEnumerator ShowDefeatPanel()
    {
        // On affiche le panel de victoire
        DefeatPanel.SetActive(true);

        // On attendq 3 secondes
        yield return new WaitForSeconds(PanelTimer);

        // On masque le panel
        DefeatPanel.SetActive(false);
    }

    private string GetStatsText(Player player, int round)
    {
        return $"Point(s) de vie : {player.LifePoint}\n"
            + $"Score : {player.ScorePoint}\n"
            + $"Round : {round + 1}\n";
    }

}
