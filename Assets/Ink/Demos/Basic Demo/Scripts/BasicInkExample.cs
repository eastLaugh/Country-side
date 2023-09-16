using System;
using System.Collections;
using System.Linq;
using Ink.Runtime;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// This is a super bare bones example of how to play and display a ink story in Unity.
public class BasicInkExample : MonoBehaviour
{
	public static event Action<Story> OnCreateStory;

	bool LOCKED = false;
	private void Awake()
	{
		Story = new Story(inkJSONAsset.text);
		Story.BindExternalFunction("InfoWindowCreate", (string text) =>
		{
			InfoWindow.Create(text);
			return null;
		});

		Story.BindExternalFunction("LOCK", LOCK);
		Story.BindExternalFunction("Assign", (string text) =>
		{
			if (text == "Building3AdobeHouses")
			{
				Debug.Log(LOCKED);
				StartCoroutine(Wait3s());
				IEnumerator Wait3s()
				{
					yield return new WaitForSeconds(3);
					LOCK(); //假设任务已完成，再次调用LOCK ，解锁按钮
				}
			}
		});
	}

	void LOCK()
	{
		LOCKED = !LOCKED;
		foreach (Button btn in panel.GetComponentsInChildren<Button>())
		{
			btn.interactable = !LOCKED;
		}
	}

	void Start()
	{
		// Remove the default message
		RemoveChildren();
		StartStory();
	}

	// Creates a new Story object with the compiled story which we can then play!
	void StartStory()
	{
		OnCreateStory?.Invoke(Story);
		RefreshView();
	}

	// This is the main function called every time the story changes. It does a few things:
	// Destroys all the old content and choices.
	// Continues over all the lines of text, then displays all the choices. If there are no choices, the story is finished!
	void RefreshView()
	{
		// Remove all the UI on screen
		RemoveChildren();

		// Read all the content until we can't continue any more
		while (Story.canContinue)
		{
			// Continue gets the next line of the story
			string text = Story.Continue();
			// This removes any white space from the text.
			text = text.Trim();
			// Display the text on screen!
			CreateContentView(text);
		}

		// Display all the choices, if there are any!
		if (Story.currentChoices.Count > 0)
		{
			for (int i = 0; i < Story.currentChoices.Count; i++)
			{
				Choice choice = Story.currentChoices[i];
				Button button = CreateChoiceView(choice.text.Trim());
				// Tell the button what to do when we press it
				button.onClick.AddListener(delegate
				{
					OnClickChoiceButton(choice);
				});
			}
		}
		// If we've read all the content and there's no choices, the story is finished!
		else
		{
			// Button choice = CreateChoiceView("End of story.\nRestart?");
			// choice.onClick.AddListener(delegate
			// {
			// 	StartStory();
			// });
			panel.SetActive(false);
		}
	}

	// When we click the choice button, tell the story to choose that choice!
	void OnClickChoiceButton(Choice choice)
	{
		Story.ChooseChoiceIndex(choice.index);
		RefreshView();
	}

	// Creates a textbox showing the the line of text
	void CreateContentView(string text)
	{
		TextMeshProUGUI storyText = Instantiate(textPrefab).GetComponent<TextMeshProUGUI>();
		storyText.text = text;
		storyText.transform.SetParent(panel.transform, false);
		storyText.gameObject.SetActive(true);
	}

	// Creates a button showing the choice text
	Button CreateChoiceView(string text)
	{
		// Creates the button from a prefab
		Button choice = Instantiate(buttonPrefab).GetComponent<Button>();
		choice.transform.SetParent(panel.transform, false);
		choice.interactable = !LOCKED;

		// Gets the text from the button prefab
		TextMeshProUGUI choiceText = choice.GetComponentInChildren<TextMeshProUGUI>();
		choiceText.text = text;

		// Make the button expand to fit the text
		HorizontalLayoutGroup layoutGroup = choice.GetComponent<HorizontalLayoutGroup>();
		layoutGroup.childForceExpandHeight = false;

		choice.gameObject.SetActive(true);
		return choice;
	}

	// Destroys all the children of this gameobject (all the UI)
	void RemoveChildren()
	{
		int childCount = panel.transform.childCount;
		for (int i = childCount - 1; i >= 0; --i)
		{
			Destroy(panel.transform.GetChild(i).gameObject);
		}
	}

	[SerializeField]
	private TextAsset inkJSONAsset = null;
	public static Story Story { get; private set; }

	[SerializeField]
	private GameObject panel = null;

	// UI Prefabs
	[SerializeField]
	private GameObject textPrefab = null;
	[SerializeField]
	private GameObject buttonPrefab = null;
}
