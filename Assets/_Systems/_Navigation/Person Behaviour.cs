using System;
using System.Collections;
using System.Collections.Generic;
using Unity.AI.Navigation;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

public class PersonBehaviour : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, IPointerClickHandler
{
    Person person;
    public Outline outline;

    public Color EnterColor;
    public Color SelectedColor;
    public static PersonBehaviour SelectedPerson { get; private set; }

    public NavMeshAgent agent;
    private void Awake()
    {
        person = new Person(gameObject.name);

    }
    public void OnPointerClick(PointerEventData eventData)
    {
        if (SelectedPerson != this)
        {
            if (SelectedPerson != null)
            {
                SelectedPerson.DeSelect();
            }
            SelectedPerson = this;
            outline.OutlineColor = SelectedColor;
            outline.enabled = true;

            OnSelect();
        }
        else
        {
            SelectedPerson = null;
            DeSelect();

            OnPointerEnter(null);

        }
    }

    private void OnGUI()
    {
        if (SelectedPerson == this)
        {
            var rect = new Rect(0, 0, 200, 200);
            GUI.Box(rect, "Person");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (SelectedPerson != this)
        {
            outline.OutlineColor = EnterColor;
            outline.enabled = true;

        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (SelectedPerson != this)
        {
            outline.enabled = false;
        }
    }

    void OnSelect()
    {
        SlotRender.OnAnySlotClicked += OnAnySlotClicked;
    }

    void DeSelect()
    {
        outline.enabled = false;
        SlotRender.OnAnySlotClicked -= OnAnySlotClicked;
    }

    private void OnAnySlotClicked(SlotRender render)
    {
        DeSelect();
        person.destination = render.slot;

        agent.SetDestination(render.slot.worldPosition);
    }
}