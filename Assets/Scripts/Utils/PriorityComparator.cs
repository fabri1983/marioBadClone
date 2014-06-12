using System.Collections.Generic;

/**
 * Used for sorting the spawn position triggers
 */
public sealed class PriorityComparator : IComparer<SpawnPositionTrigger> {
	int IComparer<SpawnPositionTrigger>.Compare(SpawnPositionTrigger a, SpawnPositionTrigger b) {
		SpawnPositionTrigger va = (SpawnPositionTrigger)a;
		SpawnPositionTrigger vb = (SpawnPositionTrigger)b;
		int vaPrio = va.getSpawnPos().priority;
		int vbPrio = vb.getSpawnPos().priority;
		if (vaPrio < vbPrio)
			return -1;
		else if (vaPrio > vbPrio)
			return 1;
		return 0;
	}
}
