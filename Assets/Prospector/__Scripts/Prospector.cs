using UnityEngine;
using UnityEngine.SceneManagement;

using System.Collections;
using System.Collections.Generic;

/// <summary>
/// Prospector: The Prospector class manages the overall game. Whereas
/// Deck handles the creation of cards, Prospector turns those cards
/// into a game. Prospector collects the cards into various piles
/// (like the draw pile and discard pile) and manages game logic.
/// </summary>
public class Prospector : MonoBehaviour {

	static public Prospector 	S;

	[Header("Set in Inspector")]
	public TextAsset			deckXML;


	[Header("Set Dynamically")]
	public Deck					deck;

	void Awake(){
		S = this;
	}

	void Start() {
		deck = GetComponent<Deck> (); //get the deck
		deck.InitDeck (deckXML.text); // pass DeckXML to it
	}

}
