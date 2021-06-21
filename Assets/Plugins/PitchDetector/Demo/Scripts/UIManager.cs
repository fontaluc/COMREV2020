using FinerGames.PitchDetector;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    [SerializeField] MicrophoneInput microInput;
    [SerializeField] Graph graph;
    [SerializeField] Text score;
    [SerializeField] Text buttonText;

    private LineRenderer lineRenderer;
    private int counter = 0;

    private AudioSource source;
    private AudioClip recording;
    float startRecordingTime;

    private void Start()
    {
        lineRenderer = graph.GetComponent<LineRenderer>();
        source = GetComponent<AudioSource>();
    }

    public void ChangeText()
    {
        counter = (counter + 1)%2;
        if (counter == 1)
        {
            buttonText.text = "Arrêter";
            graph.Reset();
            score.text = "";
            microInput.IsRecording = true;

            //Start the recording, the length of 300 gives it a cap of 5 minutes
            //recording = Microphone.Start("", false, 300, 44100);
            startRecordingTime = Time.time;
        }
        else
        {
            buttonText.text = "Démarrer";
            microInput.IsRecording = false;

            //Microphone.End("");

            recording = microInput.Source.clip;
            //Trim the audioclip by the length of the recording
            AudioClip recordingNew = AudioClip.Create(recording.name, (int)((Time.time - startRecordingTime) * recording.frequency), recording.channels, recording.frequency, false);
            float[] data = new float[(int)((Time.time - startRecordingTime) * recording.frequency)];
            Debug.Log((int)((Time.time - startRecordingTime) * recording.frequency));
            recording.GetData(data, 0);
            recordingNew.SetData(data, 0);
            this.recording = recordingNew;

            source.clip = recording; 

            // Calculs 

            int numberCount = lineRenderer.positionCount;
            Vector3[] positions = new Vector3[numberCount];
            lineRenderer.GetPositions(positions);
            double[] freq = new double[numberCount];
            for (int i = 0; i < numberCount; i++)
            {
                freq[i] = positions[i].y;
            }

            // Médiane

            int halfIndex = numberCount / 2;
            List<double> sortedNumbers = freq.OrderBy(n => n).ToList();
            double median;
            if ((numberCount % 2) == 0)
            {
                median = (sortedNumbers[halfIndex] + sortedNumbers[halfIndex - 1]) / 2;
            }
            else
            {
                median = sortedNumbers[halfIndex];
            }
            Debug.Log(median);
            score.text = Math.Round(freq[numberCount - 1] - median, 1).ToString();
        }
    }

    public void Replay()
    {
        source.Play();
    }
}
