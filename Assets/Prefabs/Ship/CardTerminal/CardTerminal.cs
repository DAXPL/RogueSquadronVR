using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 Kod ten obs³uguje terminal do kart, który sprawdza ID karty oraz czas, 
 w jakim karta by³a przesuwana przez terminal. 
 Jeœli karta jest przesuwana w odpowiednim czasie i ma poprawne ID, 
 to terminal zapali wskaŸnik na zielono i wywo³a zdarzenie onApproved. 
 W przeciwnym razie, wskaŸnik zapali siê na czerwono i wywo³ane zostanie 
 zdarzenie onRejected.
 */
public class CardTerminal : MonoBehaviour
{
    // ID karty, której oczekuje terminal
    [SerializeField] private string cardId = "";

    // Renderer wskaŸnika, który zmienia kolor w zale¿noœci od wyniku autoryzacji
    [SerializeField] private MeshRenderer indicatorRenderer;

    // Oczekiwany czas przesuniêcia karty
    private float expectedCardSwipeTime = 0.5f;

    // Czas wejœcia karty do terminala
    private float cardEnterTimestamp = 0;

    // Dopuszczalny margines b³êdu dla czasu przesuniêcia
    private const float marginOfError = 0.25f;

    // Wydarzenie wywo³ywane, gdy karta zostanie zaakceptowana
    [SerializeField] private UnityEvent onApproved;

    // Wydarzenie wywo³ywane, gdy karta zostanie odrzucona
    [SerializeField] private UnityEvent onRejected;

    // Funkcja wywo³ywana, gdy obiekt wejdzie w strefê kolizji z terminalem
    private void OnTriggerEnter(Collider other)
    {
        // Zapisanie czasu wejœcia karty do terminala
        cardEnterTimestamp = Time.time;
    }

    // Funkcja wywo³ywana, gdy obiekt opuœci strefê kolizji z terminalem
    private void OnTriggerExit(Collider other)
    {
        // Sprawdzenie, czy obiekt posiada komponent Card
        if (!other.TryGetComponent(out Card card)) return;

        // Sprawdzenie, czy ID karty jest zgodne z oczekiwanym ID
        if (card.GetCardID() != cardId)
        {
            // Odrzucenie karty jeœli ID siê nie zgadza
            indicatorRenderer.material.color = Color.red;
            return;
        }
        // Obliczenie czasu przesuniêcia karty
        float swipeTime = Time.time - cardEnterTimestamp;
        Debug.Log($"Swipe time: {swipeTime}");

        // Sprawdzenie, czy czas przesuniêcia mieœci siê w dopuszczalnym zakresie
        bool swipeApproved = (swipeTime <= expectedCardSwipeTime + marginOfError) && (swipeTime >= expectedCardSwipeTime - marginOfError);

        // Wywo³anie odpowiednich wydarzeñ w zale¿noœci od wyniku autoryzacji
        if (swipeApproved)
            onApproved.Invoke();
        else
            onRejected.Invoke();

        // Ustawienie koloru wskaŸnika na zielony lub czerwony w zale¿noœci od wyniku autoryzacji
        if (indicatorRenderer != null)
            indicatorRenderer.material.color = swipeApproved ? Color.green : Color.red;
    }
}
