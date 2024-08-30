using System;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using static SetPlayers;

public class CardCreator : MonoBehaviour
{
    //Tipos de prefabs
    public GameObject card;
    public GameObject leaderCard;

    //Carta que se está creando
    private GameObject CardInstance;

    //Booleanos
    private bool isFactionNew = false; //Comprueba si ya se le asignó una facción nueva al jugador 1
    private bool isLeaderCreated = false; //Comprueba si ya existe un líder

    //Imágenes para las cartas
    public Sprite climate;
    public Sprite lure;
    public Sprite clearance;
    public Sprite goldUnit;
    public Sprite increase;
    public Sprite meleeSilver;
    public Sprite rangedSilver;
    public Sprite siegeSilver;

    //Método principal de la clase
    public void Create(List<Property> properties)
    {
        if(WhichTypeCardIS(properties) == "Leader")
        {
            if(isLeaderCreated) throw new Exception("You have already assigned a new leader and there can only be one");
            CardInstance = LeaderInstance();
            isLeaderCreated = true;
        }
        else
        {
            CardInstance = Instantiate(card, GameObject.Find("NewDeck").transform);
        } 

        //Se le añaden las características deseadas a la carta
        Card cardScript = CardInstance.GetComponent<Card>();

        SetProperties(cardScript, properties);

        //Se le establece la facción del jugador 1 como la misma que la de las cartas mientras que esta no sea la "neutral"
        if(!isFactionNew && cardScript.Faction != "Neutral")
        {
            player1.SetFaction(cardScript.Faction);
            isFactionNew = true;
        } 
        //Si ya se estableció la nueva facción se comprueba que las cartas sigan perteneciendo a la misma o sean neutrales
        else if(isFactionNew && cardScript.Faction != "Neutral" && cardScript.Faction != player1.Faction)
        {
            Destroy(CardInstance);
            throw new Exception("Cards must belong to the same faction or be neutral");
        }
        
        //Se agrega al deck si la carta no es líder
        if(cardScript.Type != "Leader")
        {
            Translator translator = GameObject.Find("Botón Confirmar").GetComponent<Translator>();

            translator.cards.Add(CardInstance);
        }

        //Se le otorga una descripción por defecto a la carta si no es de las que pueden tener efectos
        if(cardScript.Type != "Leader" && cardScript.Type != "Silver" && cardScript.Type != "Gold")
        SetDescriptionAndImage();
    }

    // Sobrecarga para el caso de cartas con efectos
     public void Create(List<Property> properties, List<Effect> effects)
     {
        Create(properties);
        foreach(Effect effect in effects) CardInstance.GetComponent<NewCard>().Effects.Add((Effect)effect.Clone());

        SetDescriptionAndImage();
     }

    //Método para agregarle las propiedades a la carta nueva
    public void SetProperties(Card card, List<Property> properties)
    {
        int count = 0;

        for(int i = 0; i < properties.Count; i++)
        {
            //Se le otorga en dependencia del tipo de la propiedad
            if(properties[i].Type == "Type") card.Type = (string)properties[i].Value;
            else if(properties[i].Type == "Name") card.Name = (string)properties[i].Value;
            else if(properties[i].Type == "Faction") card.Faction = (string)properties[i].Value;
            else if(properties[i].Type == "Power") card.Power = (int)properties[i].Value;
            else if(properties[i].Type == "Range")
            {
                //En el caso del rango se comprueba que sea uno de los establecidos y lanza una excepción de no serlo
                if((string)properties[i].Value == "Melee" || (string)properties[i].Value == "Ranged" || (string)properties[i].Value == "Siege")
                if(count==0)
                {
                    card.Range1 = (string)properties[i].Value;
                    count++;
                } 
                else if(count==1)
                {
                    card.Range2 = (string)properties[i].Value;
                    count++;
                }
                else
                {
                    card.Range3 = (string)properties[i].Value;
                    count++;
                }
                else throw new Exception("Unvalid card range");
            } 
        }
    }

    //Método para instanciar y cambiar de líder
    private GameObject LeaderInstance()
    {
        //Se instacia en la posición correcta y se elimina al líder anterior
        GameObject cardInstance = Instantiate(leaderCard, GameObject.Find("LeaderPosition").transform);

        Destroy(GameObject.Find("DovahkiinCardNordic"));

        //Se cambia el trigger del bloqueador de la carta líder
        EventTrigger eventTrigger = GameObject.Find("LeaderBlock1").GetComponent<EventTrigger>();

        EventTrigger.Entry pointerEnterEntry = null;
            
        foreach (var entry in eventTrigger.triggers)
        {
            if (entry.eventID == EventTriggerType.PointerEnter)
            {
                pointerEnterEntry = entry;
                break;
            }
        }

        void callback(BaseEventData eventData)
        {
            cardInstance.SendMessage("ViewCard");
        }

        // Se añade el nuevo trigger
        pointerEnterEntry.callback.AddListener(callback);
            
        return cardInstance;
    }

    //Método para verificar si la carta es líder o no y proceder en consecuencia en el script "Translator"
    public string WhichTypeCardIS(List<Property> properties)
    {
        foreach(Property property in properties)
        {
            if(property.Type == "Type" && (string)property.Value == "Leader") return "Leader";
            else if(property.Type == "Type") return "Other";
        }
        throw new Exception("This card has no type");
    }

    // Método para otorgar una descripción acorde a la carta creada
    private void SetDescriptionAndImage()
    {
        TMP_Text description = CardInstance.GetComponentInChildren<TMP_Text>();
        Image image = CardInstance.GetComponentsInChildren<Image>()[1];
        NewCard properties = CardInstance.GetComponent<NewCard>();
        
        if(properties.Type == "Climate")
        {
            description.text = "Carta de clima";
            image.sprite = climate;
        } 
        else if(properties.Type == "Lure")
        {
            description.text = "Carta de señuelo";
            image.sprite = lure;
        } 
        else if(properties.Type == "Increase")
        {
            description.text = "Carta de aumento";
            image.sprite = increase;
        } 
        else if(properties.Type == "Clearance")
        {
            description.text = "Carta de despeje";
            image.sprite = clearance;
        } 
        else if(properties.Type == "Gold")
        {
            description.text = "Carta de unidad de oro";
            image.sprite = goldUnit;
        } 
        else if(properties.Type == "Silver")
        {
            description.text = "Carta de unidad de plata";

            if(properties.Range1 == "Melee") image.sprite = meleeSilver;
            else if(properties.Range1 == "Ranged") image.sprite = rangedSilver;
            else if(properties.Range1 == "Siege") image.sprite = siegeSilver;
        } 
        else if(properties.Type == "Leader") description.text = "Carta de líder";
        

        // De tener efectos, se informa en la descripción cuáles son
        if(properties.Effects.Count > 0)
        {
            description.text += "\n Efectos:";
            
            int count = 0;

            for(int i = 0; i < properties.Effects.Count; i++)
            {
                description.text += $"\n - \"{properties.Effects[i].Name}\"";

                count++;

                for(Effect effect = properties.Effects[i]; effect.PostEffect != null;)
                {
                    description.text += $"\n - \"{effect.PostEffect.Name}\"";

                    effect = effect.PostEffect;

                    count++;

                    if(count == 2) break;
                }

                // Si hay más de 2 efectos entonces se para y se especifíca que hay más
                if(count == 2)
                {
                    if(properties.Effects.Count - i > 1) description.text += "\n y más...";
                    break;
                } 
            }
        }
    }
}