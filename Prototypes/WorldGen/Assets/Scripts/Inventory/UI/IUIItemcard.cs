/*
    The base for all draggable card object types.
        Since I don't know what the cards should look like (if they even will be cards)
        this class acts as an intermediary between the ui draggable and the way the card is displayed.
 */
public abstract class IUIItemcard : UIDraggable {

    public ItemCard CardData {
        get {
            return cd;
        }
        set {
            CardDataChanged(value);
            cd = value;
        }
    }

    private ItemCard cd;

    //This function should be overriden to update the card gui
    public abstract void CardDataChanged(ItemCard newCard);
}
