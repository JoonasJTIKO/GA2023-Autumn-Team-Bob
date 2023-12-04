using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace TeamBobFPS
{
    public class FootstepPlayer : MonoBehaviour
    {
        [SerializeField]
        private LayerMask groundLayer;

        private new CapsuleCollider collider;

        private void Awake()
        {
            collider = GetComponent<CapsuleCollider>();
        }

        public void PlayFootstep()
        {
            RaycastHit hit;
            if (Physics.SphereCast(transform.position, collider.radius * 0.7f,
                                -transform.up, out hit, collider.height * 0.4f, groundLayer))
            {
                int random = Random.Range(0, 3);
                EGameSFX gameSFX = EGameSFX._NULL;
                switch (hit.collider.gameObject.tag)
                {
                    case "Grass":
                        switch (random)
                        {
                            case 0:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_GRASS; break;
                            case 1:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_GRASS2; break;
                            case 2:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_GRASS3; break;
                        }
                        GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(gameSFX, hit.point, volume: 0.1f, make2D: true);
                        break;
                    case "Stone":
                        switch (random)
                        {
                            case 0:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_STONE; break;
                            case 1:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_STONE2; break;
                            case 2:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_STONE3; break;
                        }
                        GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(gameSFX, hit.point, volume: 0.1f, make2D: true);
                        break;
                    case "Wood":
                        switch (random)
                        {
                            case 0:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_WOOD; break;
                            case 1:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_WOOD2; break;
                            case 2:
                                gameSFX = EGameSFX._SFX_PLAYER_RUN_WOOD3; break;
                        }
                        GameInstance.Instance.GetAudioManager().PlayAudioAtLocation(gameSFX, hit.point, volume: 0.1f, make2D: true);
                        break;
                }
            }

        }
    }
}
