
/**
 * An exceptionally simple class for keeping a running total.
 * This makes use of no 'loops' per se.
 * 
 */
public class Average {

    int binCount;

    float[] store;
    float running;
    int index;

    public Average(int binCount) {
        if (binCount < 2) { this.binCount = 2; } else { this.binCount = binCount; }
        store = new float[this.binCount];
        index = 0;
        running = 0;
    }

    public float GetNext(float next) {
        //Convert the running average back into the count of the store array
        float oldTotal = running * (float)binCount;
        //Remove the value at the current position from the old total
        oldTotal -= store[index];
        //Store the new value at the current position in the store array
        store[index] = next;
        //Add the new value to the old total
        oldTotal += next;
        //Recalculate the average based on the old total with the old value replaced with the new value
        running = oldTotal / (float)binCount;

        index++;
        //Reset the index when it reaches the max.
        if (index >= binCount) {
            index = 0;
        }

        return running;
    }
}
