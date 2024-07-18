using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/*
 Kod ten obs�uguje terminal do kart, kt�ry sprawdza ID karty oraz czas, 
 w jakim karta by�a przesuwana przez terminal. 
 Je�li karta jest przesuwana w odpowiednim czasie i ma poprawne ID, 
 to terminal zapali wska�nik na zielono i wywo�a zdarzenie onApproved. 
 W przeciwnym razie, wska�nik zapali si� na czerwono i wywo�ane zostanie 
 zdarzenie onRejected.
 */
public class CardTerminal : MonoBehaviour
{
    // ID karty, kt�rej oczekuje terminal
    [SerializeField] private string cardId = "";

    // Renderer wska�nika, kt�ry zmienia kolor w zale�no�ci od wyniku autoryzacji
    [SerializeField] private MeshRenderer indicatorRenderer;

    // Oczekiwany czas przesuni�cia karty
    private float expectedCardSwipeTime = 0.5f;

    // Czas wej�cia karty do terminala
    private float cardEnterTimestamp = 0;

    // Dopuszczalny margines b��du dla czasu przesuni�cia
    private const float marginOfError = 0.25f;

    // Wydarzenie wywo�ywane, gdy karta zostanie zaakceptowana
    [SerializeField] private UnityEvent onApproved;

    // Wydarzenie wywo�ywane, gdy karta zostanie odrzucona
    [SerializeField] private UnityEvent onRejected;

    // Funkcja wywo�ywana, gdy obiekt wejdzie w stref� kolizji z terminalem
    private void OnTriggerEnter(Collider other)
    {
        // Zapisanie czasu wej�cia karty do terminala
        cardEnterTimestamp = Time.time;
    }

    // Funkcja wywo�ywana, gdy obiekt opu�ci stref� kolizji z terminalem
    private void OnTriggerExit(Collider other)
    {
        // Sprawdzenie, czy obiekt posiada komponent Card
        if (!other.TryGetComponent(out Card card)) return;

        // Sprawdzenie, czy ID karty jest zgodne z oczekiwanym ID
        if (card.GetCardID() != cardId)
        {
            // Odrzucenie karty je�li ID si� nie zgadza
            indicatorRenderer.material.color = Color.red;
            return;
        }
        // Obliczenie czasu przesuni�cia karty
        float swipeTime = Time.time - cardEnterTimestamp;
        Debug.Log($"Swipe time: {swipeTime}");

        // Sprawdzenie, czy czas przesuni�cia mie�ci si� w dopuszczalnym zakresie
        bool swipeApproved = (swipeTime <= expectedCardSwipeTime + marginOfError) && (swipeTime >= expectedCardSwipeTime - marginOfError);

        // Wywo�anie odpowiednich wydarze� w zale�no�ci od wyniku autoryzacji
        if (swipeApproved)
            onApproved.Invoke();
        else
            onRejected.Invoke();

        // Ustawienie koloru wska�nika na zielony lub czerwony w zale�no�ci od wyniku autoryzacji
        if (indicatorRenderer != null)
            indicatorRenderer.material.color = swipeApproved ? Color.green : Color.red;
    }
}
