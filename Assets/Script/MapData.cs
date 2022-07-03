using NaughtyAttributes;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Unity.Netcode;
using UnityEngine;

namespace RedStudio.Battle10
{
    public class MapData : MonoBehaviour
    {
        [SerializeField, BoxGroup("Players Info")] Transform _playerRoot;
        [SerializeField, BoxGroup("Players Info")] Transform[] _spawnPoints;

        [SerializeField, BoxGroup("Objects")] Transform _objectsSpawnerRoot;
        [SerializeField, BoxGroup("Objects")] Interactable[] _allInteractables;
        [SerializeField, BoxGroup("Objects")] Transform[] _objectSpawnPositions;

        [field: SerializeField] public Leaderboad LeaderboardUI { get; private set; }

        public Transform PlayerRoot => _playerRoot;
        public IEnumerable<Transform> SpawnPoints => _spawnPoints;
        public Transform ObjectsParent => _objectsSpawnerRoot;

        public IEnumerable<(PlayerNetwork, Transform)> AssignSpawnToPlayer(IEnumerable<PlayerNetwork> p)
        {
            List<Transform> spawnPoints = new List<Transform>(_spawnPoints);
            spawnPoints.Shuffle();
            foreach(var el in p.Zip(spawnPoints, (a,b) => (a,b)))
            {
                yield return el;
            }
        }

        public IEnumerable<(NetworkObject, Vector2)> InitialObjectSpawn()
        {
            foreach (var el in _objectSpawnPositions)
            {
                yield return (_allInteractables.PickRandom().GetComponent<NetworkObject>(), el.position);
            }
        }



    }
}
