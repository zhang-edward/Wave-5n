using UnityEngine;
using UnityEngine.UI;
using System.Collections;

[RequireComponent(typeof(AudioSource))]
public class ScrollingText : MonoBehaviour {

	public const string START_INVISIBLE_TAG = "<color=#00000000>";
	public const string END_INVISIBLE_TAG = "</color>";

	public Text textBox;
	public string defaultText;
	public string text;		// text to display in the text box

	private AudioSource audioSource;

	void Awake()
	{
		audioSource = GetComponent<AudioSource> ();
	}

	void OnEnable()
	{
		SetToDefaultText ();
	}

	public void SetToDefaultText()
	{
		UpdateText (defaultText);
	}

	public void UpdateText(string text)
	{
		this.text = text;
		StopAllCoroutines ();
		StartCoroutine(AnimateText());
	}

	public void Update()
	{
		if (Input.GetMouseButtonDown(0))
		{
			StopAllCoroutines ();
			textBox.text = text;
		}
	}

	IEnumerator AnimateText()
	{
		bool playSound = true;
		for (int i = 0; i < CountNonMarkupCharacters(text) + 1; i++)
		{
			textBox.text = ParseRichTextForTypewriter(i, text);
			if (playSound)
				audioSource.Play();
			playSound = !playSound;
			yield return new WaitForSeconds(.03f);
		}
	}

	/// <summary>
	/// Parses the rich text for <see cref="AnimateText"/> so that it does not write out
	/// rich text markup, but rather handles tags in the text gracefully. For example, with
	/// the text, "I <color="#0f0">love</color> processing strings!", the method would return:
	/// i = 1: "I "
	/// i = 2: "I <color="#0f0">l</color>"
	/// i = 3: "I <color="#0f0">lo</color>"
	/// ... and so on.
	/// </summary>
	/// <returns>The rich text for typewriter.</returns>
	/// <param name="i">The index.</param>
	private string ParseRichTextForTypewriter(int numChars, string str)
	{
		string answer = "";
		bool openTag = false;

		int numNonMarkupCharacters = 0;		// number of characters counted
		int i = 0;							// substring index

		while (numNonMarkupCharacters < numChars)	// continue until we have reached char index 'i', excluding markup
		{
			if (str [i] == '<')
			{
				int endOfTagIndex = str.IndexOf ('>', i);		// skip to the end of the tag
				int length = endOfTagIndex - i + 1;
				answer += str.Substring(i, length);	// add the tag to the answer
				openTag = !openTag;	// if we have an encountered an open tag <>, close it </>. 
									// Else, mark that we have found an unclosed tag.
				i = endOfTagIndex + 1;
			}
			answer += str [i];
			i++;
			numNonMarkupCharacters++;
		}
		if (openTag)	// close any open tags
		{
			int startIndex = str.IndexOf ('<', i);
			int endIndex = str.IndexOf ('>', startIndex);
			int length = endIndex - startIndex + 1;
			answer += str.Substring (startIndex, length);
		}
		return answer;
	}

	private int CountNonMarkupCharacters(string str)
	{
		int counter = 0;
		bool markup = false;
		for (int i = 0; i < str.Length; i ++)
		{
			if (str [i] == '<')		// found the beginning of a tag
			{
				markup = true;
			}
			else if (markup && str[i - 1] == '>')		// found the character after end of a tag
			{
				markup = false;
			}
			if (!markup)
			{
				counter++;
			}
		}
		return counter;
	}
}
