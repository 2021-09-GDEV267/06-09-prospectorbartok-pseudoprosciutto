using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

/// <summary>
/// Gaps: The Gaps class manages the overall game. Whereas
/// Deck handles the creation of cards, Gaps turns those cards
/// into a game. Gaps collects the cards into various piles
/// (like the draw pile and discard pile) and manages game logic.
/// </summary>
public class Gaps : MonoBehaviour
{
	static public Gaps S;

	[Header("Set in Inspector")]
	[Range(0.0f,1.0f)]
	public float randomizeGoldCards=.1f;
	public TextAsset deckXML;
	public TextAsset GapsLayout;
	public float xOffset = 3;
	public float yOffset = -2.5f;
	public Vector3 layoutCenter;

	public Vector2 fsPosMid = new Vector2(0.5f, 0.90f);
	public Vector2 fsPosRun = new Vector2(0.5f, 0.75f);
	public Vector2 fsPosMid2 = new Vector2(0.4f, 1.0f);
	public Vector2 fsPosEnd = new Vector2(0.5f, 0.95f);

	public float reloadDelay = 2f;// 2 sec delay between rounds

	public Text gameOverText, roundResultText, highScoreText;

	[Header("Set Dynamically")]
	public Deck deck;
	public Layout layout;
	public List<CardGaps> drawPile;
	public Transform layoutAnchor;
	public CardGaps target;
	public List<CardGaps> tableau;
	public List<CardGaps> discardPile;
	
	public FloatingScore fsRun;

	

	void Awake()
	{
		S = this;
		SetUpUITexts();
	}

	void Start()
	{
		Scoreboard.S.score = ScoreManager.SCORE;

		deck = GetComponent<Deck>(); //get the deck
		deck.InitDeck(deckXML.text); // pass DeckXML to it
		Deck.Shuffle(ref deck.cards);// thus shuffes deck by reference

		/** Testing Deck Layout
		Card c;
		for (int cNum=0; cNum<deck.cards.Count;cNum++)
        {
			c = deck.cards[cNum];
			c.transform.localPosition = new Vector3((cNum % 13) * 3, cNum / 13 * 4, 0);
        }
		*/

		layout = GetComponent<Layout>();  // Get the Layout component
		layout.ReadLayout(GapsLayout.text); // Pass LayoutXML to it

		drawPile = ConvertListCardsToListCardGaps(deck.cards);
		LayoutGame();
	}


	void SetUpUITexts()
	{

		// Set up the HighScore UI Text
		GameObject go = GameObject.Find("HighScore");

		if (go != null)
		{
			highScoreText = go.GetComponent<Text>();
		}

		int highScore = ScoreManager.HIGH_SCORE;
		string hScore = "High Score: " + Utils.AddCommasToNumber(highScore);
		go.GetComponent<Text>().text = hScore;

		// Set up the UI Texts that show at the end of the round

		go = GameObject.Find("GameOver");

		if (go != null)
		{
			gameOverText = go.GetComponent<Text>();
		}

		go = GameObject.Find("RoundResult");

		if (go != null)
		{
			roundResultText = go.GetComponent<Text>();
		}
		// Make the end of round texts invisible
		ShowResultsUI(false);
	}



	void ShowResultsUI(bool show)
	{
		gameOverText.gameObject.SetActive(show);
		roundResultText.gameObject.SetActive(show);
	}



	List<CardGaps> ConvertListCardsToListCardGaps(List<Card> lCD)
	{
		List<CardGaps> lCP = new List<CardGaps>();
		CardGaps tCP;
		foreach (Card tCD in lCD)
		{
			tCP = tCD as CardGaps;                              
			lCP.Add(tCP);
		}

		return (lCP);
	}

	// The Draw function will pull a single card from the drawPile and return it

	CardGaps Draw()
	{
		CardGaps cd = drawPile[0]; // Pull the 0th CardProspector
		drawPile.RemoveAt(0);            // Then remove it from List<> drawPile
		return (cd);                      // And return it
	}

	/*// Moves the current target to the discardPile
	void MoveToDiscard(CardGaps cd)
	{
		// Set the state of the card to discard
		cd.state = GapCardState.discard;
		discardPile.Add(cd); // Add it to the discardPile List<>
		cd.transform.parent = layoutAnchor; // Update its transform parent

		// Position this card on the discardPile
		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID + 0.5f);
		cd.faceUp = true;

		// Place it on top of the pile for depth sorting
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(-100 + discardPile.Count);
	}*/

	// Make cd the new target card
	void MoveToTarget(CardGaps cd)
	{
		// If there is currently a target card, move it to discardPile
	//	if (target != null) MoveToDiscard(target);
		target = cd; // cd is the new target
		cd.state = GapCardState.target;
		cd.transform.parent = layoutAnchor;
		// Move to the target position

		cd.transform.localPosition = new Vector3(
			layout.multiplier.x * layout.discardPile.x,
			layout.multiplier.y * layout.discardPile.y,
			-layout.discardPile.layerID);

		cd.faceUp = true; // Make it face-up

		// Set the depth sorting
		cd.SetSortingLayerName(layout.discardPile.layerName);
		cd.SetSortOrder(0);
	}



	// Arranges all the cards of the drawPile to show how many are left
	void UpdateDrawPile()
	{

		
		CardGaps cd;

		// Go through all the cards of the drawPile
		for (int i = 0; i < drawPile.Count; i++)
		{
			cd = drawPile[i];
			cd.transform.parent = layoutAnchor;

			// Position it correctly with the layout.drawPile.stagger
			Vector2 dpStagger = layout.drawPile.stagger;

			cd.transform.localPosition = new Vector3(
				layout.multiplier.x * (layout.drawPile.x + i * dpStagger.x),
				layout.multiplier.y * (layout.drawPile.y + i * dpStagger.y),
				-layout.drawPile.layerID + 0.1f * i);

			cd.faceUp = false; // Make them all face-down
		//	cd.state = GapCardState.drawpile;    No Draw Pile
			// Set depth sorting
			cd.SetSortingLayerName(layout.drawPile.layerName);
			cd.SetSortOrder(-10 * i);
		}
	}


	// LayoutGame() positions the initial tableau of cards, a.k.a. the "mine"
	void LayoutGame()
	{
		// Create an empty GameObject to serve as an anchor for the tableau // a
		if (layoutAnchor == null)
		{
			GameObject tGO = new GameObject("_LayoutAnchor");
			// ^ Create an empty GameObject named _LayoutAnchor in the Hierarchy
			layoutAnchor = tGO.transform;              // Grab its Transform
			layoutAnchor.transform.position = layoutCenter;   // Position it
		}

		CardGaps cp;
		// Follow the layout
		//Draw all 52 cards
		foreach (SlotDef tSD in layout.slotDefs)
		{
			// ^ Iterate through all the SlotDefs in the layout.slotDefs as tSD
			cp = Draw(); // Pull a card from the top (beginning) of the draw Pile
			cp.faceUp = tSD.faceUp;  // Set its faceUp to the value in SlotDef
			cp.transform.parent = layoutAnchor; // Make its parent layoutAnchor
			// This replaces the previous parent: deck.deckAnchor, which

			//  appears as _Deck in the Hierarchy when the scene is playing.
			cp.transform.localPosition = new Vector3(
				layout.multiplier.x * tSD.x,
				layout.multiplier.y * tSD.y,
				-tSD.layerID);
			// ^ Set the localPosition of the card based on slotDef

			cp.layoutID = tSD.id;
			cp.slotDef = tSD;
			// CardProspectors in the tableau have the state CardState.tableau
			cp.state = GapCardState.tableau;

			cp.SetSortingLayerName(tSD.layerName); // Set the sorting layers

			/**
			//set cards to gold
			if (Random.value <= randomizeGoldCards)
			{
				cp.isGoldCard = true;
				cp.back.GetComponent<SpriteRenderer>().sprite = deck.cardBackGold;
				cp.GetComponent<SpriteRenderer>().sprite = deck.cardFrontGold;
			}
			//cp.front.GetComponent<SpriteRenderer>().sprite = deck.cardBackGold;
			*/
			tableau.Add(cp); // Add this CardProspector to the List<> tableau    
		}

		// Set which cards are hiding others
		foreach (CardGaps tCP in tableau)
		{
			foreach (int hid in tCP.slotDef.hiddenBy)
			{
				cp = FindCardByLayoutID(hid);
				tCP.hiddenBy.Add(cp);
			}
		}
		// Set up the initial target card
		MoveToTarget(Draw());
		// Set up the Draw pile
		UpdateDrawPile();
	}

	// Convert from the layoutID int to the CardProspector with that ID

	CardGaps FindCardByLayoutID(int layoutID)
	{
		foreach (CardGaps tCP in tableau)
		{
			// Search through all cards in the tableau List<>
			if (tCP.layoutID == layoutID)
			{
				// If the card has the same ID, return it
				return (tCP);
			}
		}
		// If it's not found, return null
		return (null);
	}

	// This turns cards in the Mine face-up or face-down
	void SetTableauFaces()
	{
		foreach (CardGaps cd in tableau)
		{
			bool faceUp = true; // Assume the card will be face-up
			foreach (CardGaps cover in cd.hiddenBy)
			{
				// If either of the covering cards are in the tableau
				if (cover.state == GapCardState.tableau)
				{
					faceUp = false; // then this card is face-down
				}
			}

			cd.faceUp = faceUp; // Set the value on the card

		}
	}


/// <summary>
/// CardClicked is called any time a card in the game is clicked
/// </summary>
/// <param name="cd">card prospector</param>
	public void CardClicked(CardGaps cd)
	{
		// The reaction is determined by the state of the clicked card
		switch (cd.state)
		{
			case GapCardState.target:
				// Clicking the target card does nothing
				break;



			case GapCardState.tableau:
				// Clicking a card in the tableau will check if it's a valid play
				bool validMatch = true;
				if (!cd.faceUp)
				{
					// If the card is face-down, it's not valid
					validMatch = false;
				}

				if (!AdjacentRank(cd, target))
				{
					// If it's not an adjacent rank, it's not valid
					validMatch = false;
				}

				if (!validMatch) return; // return if not valid

				// If we got here, then: Yay! It's a valid card.
				tableau.Remove(cd); // Remove it from the tableau List
				MoveToTarget(cd);  // Make it the target card
								   // Clicking a card in the tableau will check if it's a valid play
				SetTableauFaces(); // Update tableau card face-ups

				if (cd.isGoldCard)
				{
					ScoreManager.EVENT(eScoreEvent.mineGold);
					FloatingScoreHandler(eScoreEvent.mine);
				}
				else
				{
					ScoreManager.EVENT(eScoreEvent.mine);
					FloatingScoreHandler(eScoreEvent.mine);
				}
				break;

		}
		// Check to see whether the game is over or not
		CheckForGameOver();
	}



	// Test whether the game is over

	void CheckForGameOver()
	{
		// If the tableau is empty, the game is over
		if (tableau.Count == 0)
		{
			// Call GameOver() with a win
			GameOver(true);
			return;
		}

		// If there are still cards in the draw pile, the game's not over
		if (drawPile.Count > 0)
		{
			return;
		}

		// Check for remaining valid plays
		foreach (CardGaps cd in tableau)
		{
			if (AdjacentRank(cd, target))
			{
				// If there is a valid play, the game's not over
				return;
			}
		}

		// Since there are no valid plays, the game is over
		// Call GameOver with a loss
		GameOver(false);
	}

	// Called when the game is over. Simple for now, but expandable
	void GameOver(bool won)
	{
		int score = ScoreManager.SCORE;
		if (fsRun != null) score += fsRun.score;

		if (won)
		{
			gameOverText.text = "Round Over";
			roundResultText.text = "You won this round!\nRound Score: " + score;
			ShowResultsUI(true);

			//print("Game Over. You won! :)");
			ScoreManager.EVENT(eScoreEvent.gameWin);
			FloatingScoreHandler(eScoreEvent.gameWin);
		}
		else
		{
			gameOverText.text = "Game Over";
			if (ScoreManager.HIGH_SCORE <= score)
			{
				string str = "You got the high score!\nHigh score: " + score;
				roundResultText.text = str;
			}
			else
			{
				roundResultText.text = "Your final score was: " + score;
			}

			ShowResultsUI(true);
			//print("Game Over. You Lost. :(");
			ScoreManager.EVENT(eScoreEvent.gameLoss);
			FloatingScoreHandler(eScoreEvent.gameLoss);
		}
		Invoke("ReloadLevel", reloadDelay);
	}

	void ReloadLevel()
	{

		// Reload the scene, resetting the game

		SceneManager.LoadScene("__Prospector_Scene_0");

	}


	// Return true if the two cards are adjacent in rank (A & K wrap around)
	public bool AdjacentRank(CardGaps c0, CardGaps c1)
	{
		// If either card is face-down, it's not adjacent.
		if (!c0.faceUp || !c1.faceUp) return (false);

		// If they are 1 apart, they are adjacent
		if (Mathf.Abs(c0.rank - c1.rank) == 1)
		{
			return (true);
		}

		// If one is Ace and the other King, they are adjacent
		if (c0.rank == 1 && c1.rank == 13) return (true);
		if (c0.rank == 13 && c1.rank == 1) return (true);

		// Otherwise, return false
		return (false);
	}

	// Handle FloatingScore movement

	void FloatingScoreHandler(eScoreEvent evt)
	{
		List<Vector2> fsPts;

		switch (evt)
		{
			// Same things need to happen whether it's a draw, a win, or a loss
			case eScoreEvent.draw:     // Drawing a card
			case eScoreEvent.gameWin:  // Won the round
			case eScoreEvent.gameLoss: // Lost the round

				// Add fsRun to the Scoreboard score

				if (fsRun != null)
				{
					// Create points for the Bézier curve1
					fsPts = new List<Vector2>();
					fsPts.Add(fsPosRun);
					fsPts.Add(fsPosMid2);
					fsPts.Add(fsPosEnd);

					fsRun.reportFinishTo = Scoreboard.S.gameObject;
					fsRun.Init(fsPts, 0, 1);

					// Also adjust the fontSize
					fsRun.fontSizes = new List<float>(new float[] { 28, 36, 4 });
					fsRun = null; // Clear fsRun so it's created again
				}
				break;

			case eScoreEvent.mine: // Remove a mine card
				// Create a FloatingScore for this score
				FloatingScore fs;
				// Move it from the mousePosition to fsPosRun
				Vector2 p0 = Input.mousePosition;
				p0.x /= Screen.width;
				p0.y /= Screen.height;
				fsPts = new List<Vector2>();
				fsPts.Add(p0);
				fsPts.Add(fsPosMid);
				fsPts.Add(fsPosRun);

				fs = Scoreboard.S.CreateFloatingScore(ScoreManager.CHAIN, fsPts);

				fs.fontSizes = new List<float>(new float[] { 4, 50, 28 });

				if (fsRun == null)
				{
					fsRun = fs;
					fsRun.reportFinishTo = null;
				}
				else
				{
					fs.reportFinishTo = fsRun.gameObject;
				}
				break;


		}
	}
}