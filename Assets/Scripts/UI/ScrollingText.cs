using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ScrollingText : MonoBehaviour {

	public Text textBox;
	public string defaultText;
	public string text;		// text to display in the text box
	public AudioClip scrollingTextSound;

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
		for (int i = 0; i < CountNonMarkupCharacters(text) + 1; i++)
		{
			textBox.text = ParseRichTextForTypewriter(i, text);
			SoundManager.instance.PlaySingle (scrollingTextSound);
			yield return new WaitForSeconds(.05f);
		}
	}

	/// <summary>
	/// Parses the rich text for <see cref="AnimateText"/> so that it does not write out
	/// rich text markup, but rather handles tags in the text gracefully. For example, with
	/// the text, "I <color="#0f0">love</color> processing strings!", the method would return:
	/// i = 1: "I"
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

		int numNonMarkupCharacters = 0;
		int i = 0;
		while (numNonMarkupCharacters < numChars)	// continue until we have reached char index 'i', excluding markup
		{
			if (str [i] == '<')
			{
				int endOfTagIndex = str.IndexOf ('>', i);		// skip to the end of the tag
				int length = endOfTagIndex - i + 1;
				answer += str.Substring(i, length);	// add the tag to the answer
				openTag = !openTag;	// if we have an encountered a tag, close it. Else, mark that we have found an unclosed tag
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
