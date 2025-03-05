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
     * 1) On charge tout les gameobjects AR sur la sc�ne � partir du QuestionsSet dans une liste GO AR
     * 2) On lance ShowNextQuestion()
     * 
     * ShowNextQuestion() :
     * 3) On charge la prochaine question :
     *      On modifie le texte de la question affich�e � partir du QuestionSet(currentRound)
     *      On modifie le texte des stats du joueur
     *      
     *      Pour tout les GO AR de la liste GO AR :
     *          On enl�ve tout ses listeners
     *          Si l'index du GO AR courant �gal le currentRound :
     *              On ajoute un listener de WinRound()
     *          Sinon
     *              On ajoute un listener de LostRound()
     * 
     *      
     * WinRound() :
     *      Pour tout les GO AR de la liste GO AR :
     *          On enl�ve tout ses listeners
     *          
     *      On affiche un message de victoire
     *      (On joue un son)
     *      On ajoute un point de score au joueur
     *      On augmente de un le currentRound
     *      
     *      On attends x secondes (si possible afficher un d�compte)
     *      
     *      On lance ShowNextQuestion()
     * 
     * LostRound() :
     *      (On enl�ve tout les listeners du GO AR qui a lanc� l'event)
     * 
     *      On affiche un message de d�faite
     *      (On joue un son)
     *      On enl�ve un point de vie au joueur
     *      
     *      Si le joueur n'a plus de points de vie :
     *          On lance la Sc�ne de d�faite
     *          
     *      On attends x secondes (si possible afficher un d�compte)
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
    /// Liste des objets instanci�s sur la sc�ne
    /// </summary>
    [SerializeField] private List<GameObject> spawnedQuestions = new List<GameObject>();

    void Start()
    {
        try
        {
            // On charge les questions sur la sc�ne
            LoadQuestions();

            // On d�sactive le panneau de Victoire et de D�faite
            VictoryPanel.SetActive(false);
            DefeatPanel.SetActive(false);

            // On copie les presets joueur sur le joueur courant
            currentPlayer = new Player();
            currentPlayer.SetLifePoint(PlayerSet.LifePoint);
            currentPlayer.SetScorePoint(PlayerSet.ScorePoint);

            // On affiche la premi�re question
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
            // Nettoyer la sc�ne en supprimant les questions pr�c�dentes (si besoin)
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
            // Si il y a encore des questions dans le QuestionsSet � l'index courant :
            if (currentQuestionIndex < QuestionsSet.QuestionsList.Count)
            {
                // On mets � jour le texte des composants Question et Stats
                QuestionText.text = QuestionsSet.QuestionsList[currentQuestionIndex].Text;
                StatsText.text = GetStatsText(currentPlayer, currentQuestionIndex);

                // Pour chaque GO AR sur la sc�ne :
                foreach (GameObject gameObjectAR in spawnedQuestions)
                {
                    // On r�cup�re le composant DefaultObserverEventHandler que Vuforia utilise
                    DefaultObserverEventHandler eventHandler = gameObjectAR.GetComponent<DefaultObserverEventHandler>();

                    // On enl�ve tout les listeners sur OnTargetFound()
                    eventHandler.OnTargetFound.RemoveAllListeners();

                    // Si l'index dans la liste du GO AR correspond � l'index de la question, on ajoute WinRound() sinon LostRound()
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
                // Pour chaque GO AR sur la sc�ne, on enl�ve les listeners sur OnTargetFound() :
                foreach (GameObject gameObjectAR in spawnedQuestions)
                    gameObjectAR.GetComponent<DefaultObserverEventHandler>().OnTargetFound.RemoveAllListeners();
            }

            // On ajoute un point de score au joueur
            currentPlayer.IncreaseScorePoint();

            // On mets � jour les stats du joueur sur l'�cran
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
            // On enl�ve tout les listeners du GO AR qui a lanc� l'event
            eventHandler.OnTargetFound.RemoveAllListeners();

            // On enl�ve un point de vie au joueur
            currentPlayer.DecreaseLifePoint();

            // On mets � jour les stats du joueur sur l'�cran
            StatsText.text = GetStatsText(currentPlayer, currentQuestionIndex);

            // Si le joueur n'a plus de points de vie :
            if (currentPlayer.LifePoint <= 0)
            {
                // Il n'y a plus de points de vie => Fin de la partie
                SceneManager.LoadScene("Menu");
            }

            // (On joue un son)

            // On affiche un message de d�faite
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

        // On passe � la question suivante
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
