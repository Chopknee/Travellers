using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface IUIDropZone {
    bool TryDropItem(UIDraggable item);
    bool TryGrabItem(UIDraggable item);
    void TransferComplete(UIDraggable item);
}
