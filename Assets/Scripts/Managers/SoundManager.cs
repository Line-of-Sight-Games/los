using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Audio;

public class SoundManager : MonoBehaviour
{
    public MainGame game;
    public bool banzaiPlayed, isMute;

    //audio stuff
    public AudioSource audioSource;
    private Dictionary<AudioClip, Coroutine> playingSounds = new();

    //meta sfx
    public AudioClip banzai, overrideAlarm, detectionAlarm, buttonPress, levelUp, overmoveAlarm, gameOverMusic, failAbilityUpgrade, succeedAbilityUpgrade, newAbilityUpgrade;

    //game sfx
    public AudioClip explosion;
    public AudioClip meleeCounter, meleeBreakeven, meleeSuccessStatic, meleeSuccessCharge;
    public AudioClip placeClaymore;
    public AudioClip dipelecFail, dipelecHacking, dipelecNegotiating, dipelecSuccessL1, dipelecSuccessL2, dipelecSuccessL3, dipelecSuccessL4;
    public AudioClip shotAR, shotLMG, shotPi, shotRi, shotSh, shotSMG, shotSn, shotSuppressLMG_SMG_AR, shotSuppressPi_Ri_Sn_Sh, shotSilencedLMG_SMG_AR, shotSilencedPi_Ri_Sn_Sh, coverDestruction, reloadAR, reloadLMG, reloadPi, reloadRi, reloadSh, reloadSMG, reloadSn;
    public AudioClip fallFromHeight, structuralCollapse;
    public AudioClip equipArmour, equipWearableGear, configureGeneral;
    public AudioClip itemUseFrag, itemUseFlash, itemUseSmoke, itemUseTabun;
    public AudioClip itemUseDepBeacon, itemUseETool, itemUseFood, itemUseMedikit, itemUsePoisonSatchel, itemUseSyringe, itemUseWater, itemUsePoisonedItem;

    //dialogue
    public AudioClip[] commanderConfirmMove, spartanConfirmMove, survivorConfirmMove, runnerConfirmMove, evaderConfirmMove, reservistConfirmMove, seekerConfirmMove, chameleonConfirmMove, scoutConfirmMove, infantrymanConfirmMove, operatorConfirmMove, earthquakeConfirmMove, hunterConfirmMove, cycloneConfirmMove, hammerConfirmMove, wolfConfirmMove, herculesConfirmMove, diplomatConfirmMove, technicianConfirmMove, medicConfirmMove;
    public AudioClip[] commanderConfigNearGB, spartanConfigNearGB, survivorConfigNearGB, runnerConfigNearGB, evaderConfigNearGB, reservistConfigNearGB, seekerConfigNearGB, chameleonConfigNearGB, scoutConfigNearGB, infantrymanConfigNearGB, operatorConfigNearGB, earthquakeConfigNearGB, hunterConfigNearGB, cycloneConfigNearGB, hammerConfigNearGB, wolfConfigNearGB, herculesConfigNearGB, diplomatConfigNearGB, technicianConfigNearGB, medicConfigNearGB;
    public AudioClip[] commanderDetectClaymore, spartanDetectClaymore, survivorDetectClaymore, runnerDetectClaymore, evaderDetectClaymore, reservistDetectClaymore, seekerDetectClaymore, chameleonDetectClaymore, scoutDetectClaymore, infantrymanDetectClaymore, operatorDetectClaymore, earthquakeDetectClaymore, hunterDetectClaymore, cycloneDetectClaymore, hammerDetectClaymore, wolfDetectClaymore, herculesDetectClaymore, diplomatDetectClaymore, technicianDetectClaymore, medicDetectClaymore;
    public AudioClip[] commanderEnterOverwatch, spartanEnterOverwatch, survivorEnterOverwatch, runnerEnterOverwatch, evaderEnterOverwatch, reservistEnterOverwatch, seekerEnterOverwatch, chameleonEnterOverwatch, scoutEnterOverwatch, infantrymanEnterOverwatch, operatorEnterOverwatch, earthquakeEnterOverwatch, hunterEnterOverwatch, cycloneEnterOverwatch, hammerEnterOverwatch, wolfEnterOverwatch, herculesEnterOverwatch, diplomatEnterOverwatch, technicianEnterOverwatch, medicEnterOverwatch;
    public AudioClip[] commanderHealAlly, spartanHealAlly, survivorHealAlly, runnerHealAlly, evaderHealAlly, reservistHealAlly, seekerHealAlly, chameleonHealAlly, scoutHealAlly, infantrymanHealAlly, operatorHealAlly, earthquakeHealAlly, hunterHealAlly, cycloneHealAlly, hammerHealAlly, wolfHealAlly, herculesHealAlly, diplomatHealAlly, technicianHealAlly, medicHealAlly;
    public AudioClip[] commanderEquipArmour, spartanEquipArmour, survivorEquipArmour, runnerEquipArmour, evaderEquipArmour, reservistEquipArmour, seekerEquipArmour, chameleonEquipArmour, scoutEquipArmour, infantrymanEquipArmour, operatorEquipArmour, earthquakeEquipArmour, hunterEquipArmour, cycloneEquipArmour, hammerEquipArmour, wolfEquipArmour, herculesEquipArmour, diplomatEquipArmour, technicianEquipArmour, medicEquipArmour;
    public AudioClip[] commanderPlaceClaymore, spartanPlaceClaymore, survivorPlaceClaymore, runnerPlaceClaymore, evaderPlaceClaymore, reservistPlaceClaymore, seekerPlaceClaymore, chameleonPlaceClaymore, scoutPlaceClaymore, infantrymanPlaceClaymore, operatorPlaceClaymore, earthquakePlaceClaymore, hunterPlaceClaymore, cyclonePlaceClaymore, hammerPlaceClaymore, wolfPlaceClaymore, herculesPlaceClaymore, diplomatPlaceClaymore, technicianPlaceClaymore, medicPlaceClaymore;
    public AudioClip[] commanderUseGrenade, spartanUseGrenade, survivorUseGrenade, runnerUseGrenade, evaderUseGrenade, reservistUseGrenade, seekerUseGrenade, chameleonUseGrenade, scoutUseGrenade, infantrymanUseGrenade, operatorUseGrenade, earthquakeUseGrenade, hunterUseGrenade, cycloneUseGrenade, hammerUseGrenade, wolfUseGrenade, herculesUseGrenade, diplomatUseGrenade, technicianUseGrenade, medicUseGrenade;
    public AudioClip[] commanderUseTabun, spartanUseTabun, survivorUseTabun, runnerUseTabun, evaderUseTabun, reservistUseTabun, seekerUseTabun, chameleonUseTabun, scoutUseTabun, infantrymanUseTabun, operatorUseTabun, earthquakeUseTabun, hunterUseTabun, cycloneUseTabun, hammerUseTabun, wolfUseTabun, herculesUseTabun, diplomatUseTabun, technicianUseTabun, medicUseTabun;
    public AudioClip[] commanderUseSmoke, spartanUseSmoke, survivorUseSmoke, runnerUseSmoke, evaderUseSmoke, reservistUseSmoke, seekerUseSmoke, chameleonUseSmoke, scoutUseSmoke, infantrymanUseSmoke, operatorUseSmoke, earthquakeUseSmoke, hunterUseSmoke, cycloneUseSmoke, hammerUseSmoke, wolfUseSmoke, herculesUseSmoke, diplomatUseSmoke, technicianUseSmoke, medicUseSmoke;
    public AudioClip[] commanderKillEnemy, spartanKillEnemy, survivorKillEnemy, runnerKillEnemy, evaderKillEnemy, reservistKillEnemy, seekerKillEnemy, chameleonKillEnemy, scoutKillEnemy, infantrymanKillEnemy, operatorKillEnemy, earthquakeKillEnemy, hunterKillEnemy, cycloneKillEnemy, hammerKillEnemy, wolfKillEnemy, herculesKillEnemy, diplomatKillEnemy, technicianKillEnemy, medicKillEnemy;
    public AudioClip[] commanderMeleeBreakeven, spartanMeleeBreakeven, survivorMeleeBreakeven, runnerMeleeBreakeven, evaderMeleeBreakeven, reservistMeleeBreakeven, seekerMeleeBreakeven, chameleonMeleeBreakeven, scoutMeleeBreakeven, infantrymanMeleeBreakeven, operatorMeleeBreakeven, earthquakeMeleeBreakeven, hunterMeleeBreakeven, cycloneMeleeBreakeven, hammerMeleeBreakeven, wolfMeleeBreakeven, herculesMeleeBreakeven, diplomatMeleeBreakeven, technicianMeleeBreakeven, medicMeleeBreakeven;
    public AudioClip[] commanderMeleeMove, spartanMeleeMove, survivorMeleeMove, runnerMeleeMove, evaderMeleeMove, reservistMeleeMove, seekerMeleeMove, chameleonMeleeMove, scoutMeleeMove, infantrymanMeleeMove, operatorMeleeMove, earthquakeMeleeMove, hunterMeleeMove, cycloneMeleeMove, hammerMeleeMove, wolfMeleeMove, herculesMeleeMove, diplomatMeleeMove, technicianMeleeMove, medicMeleeMove;
    public AudioClip[] commanderShotMiss, spartanShotMiss, survivorShotMiss, runnerShotMiss, evaderShotMiss, reservistShotMiss, seekerShotMiss, chameleonShotMiss, scoutShotMiss, infantrymanShotMiss, operatorShotMiss, earthquakeShotMiss, hunterShotMiss, cycloneShotMiss, hammerShotMiss, wolfShotMiss, herculesShotMiss, diplomatShotMiss, technicianShotMiss, medicShotMiss;
    public AudioClip[] commanderPickupUHF, spartanPickupUHF, survivorPickupUHF, runnerPickupUHF, evaderPickupUHF, reservistPickupUHF, seekerPickupUHF, chameleonPickupUHF, scoutPickupUHF, infantrymanPickupUHF, operatorPickupUHF, earthquakePickupUHF, hunterPickupUHF, cyclonePickupUHF, hammerPickupUHF, wolfPickupUHF, herculesPickupUHF, diplomatPickupUHF, technicianPickupUHF, medicPickupUHF;
    public AudioClip[] commanderPickupULF, spartanPickupULF, survivorPickupULF, runnerPickupULF, evaderPickupULF, reservistPickupULF, seekerPickupULF, chameleonPickupULF, scoutPickupULF, infantrymanPickupULF, operatorPickupULF, earthquakePickupULF, hunterPickupULF, cyclonePickupULF, hammerPickupULF, wolfPickupULF, herculesPickupULF, diplomatPickupULF, technicianPickupULF, medicPickupULF;
    public AudioClip[] commanderSeeEnemy, spartanSeeEnemy, survivorSeeEnemy, runnerSeeEnemy, evaderSeeEnemy, reservistSeeEnemy, seekerSeeEnemy, chameleonSeeEnemy, scoutSeeEnemy, infantrymanSeeEnemy, operatorSeeEnemy, earthquakeSeeEnemy, hunterSeeEnemy, cycloneSeeEnemy, hammerSeeEnemy, wolfSeeEnemy, herculesSeeEnemy, diplomatSeeEnemy, technicianSeeEnemy, medicSeeEnemy;
    public AudioClip[] commanderSuppressEnemy, spartanSuppressEnemy, survivorSuppressEnemy, runnerSuppressEnemy, evaderSuppressEnemy, reservistSuppressEnemy, seekerSuppressEnemy, chameleonSuppressEnemy, scoutSuppressEnemy, infantrymanSuppressEnemy, operatorSuppressEnemy, earthquakeSuppressEnemy, hunterSuppressEnemy, cycloneSuppressEnemy, hammerSuppressEnemy, wolfSuppressEnemy, herculesSuppressEnemy, diplomatSuppressEnemy, technicianSuppressEnemy, medicSuppressEnemy;
    public AudioClip[] ulfFail, ulfSuccess, uhfDialUp, uhfMiss, uhfHit, uhfDirectHit;

    public List<string> soldierTakenDamage, soldierAllyKilledJA, soldierAllyKilledOrUncon, soldierEnemyUseULF, soldierEnemyUseUHF, soldierEnemyUseTabun, soldierEnemyUseSmoke, soldierHeardSound, soldierEnemyKilledJA, soldierEnemyKilled, soldierAllyMissesShot, soldierEnemyMissesShot, soldierAllyUseULF, soldierAllyUseUHF;
    public AudioClip[] commanderSelectionAfterDamage, spartanSelectionAfterDamage, survivorSelectionAfterDamage, runnerSelectionAfterDamage, evaderSelectionAfterDamage, reservistSelectionAfterDamage, seekerSelectionAfterDamage, chameleonSelectionAfterDamage, scoutSelectionAfterDamage, infantrymanSelectionAfterDamage, operatorSelectionAfterDamage, earthquakeSelectionAfterDamage, hunterSelectionAfterDamage, cycloneSelectionAfterDamage, hammerSelectionAfterDamage, wolfSelectionAfterDamage, herculesSelectionAfterDamage, diplomatSelectionAfterDamage, technicianSelectionAfterDamage, medicSelectionAfterDamage;
    public AudioClip[] commanderSelectionAfterAllyKilledJA, spartanSelectionAfterAllyKilledJA, survivorSelectionAfterAllyKilledJA, runnerSelectionAfterAllyKilledJA, evaderSelectionAfterAllyKilledJA, reservistSelectionAfterAllyKilledJA, seekerSelectionAfterAllyKilledJA, chameleonSelectionAfterAllyKilledJA, scoutSelectionAfterAllyKilledJA, infantrymanSelectionAfterAllyKilledJA, operatorSelectionAfterAllyKilledJA, earthquakeSelectionAfterAllyKilledJA, hunterSelectionAfterAllyKilledJA, cycloneSelectionAfterAllyKilledJA, hammerSelectionAfterAllyKilledJA, wolfSelectionAfterAllyKilledJA, herculesSelectionAfterAllyKilledJA, diplomatSelectionAfterAllyKilledJA, technicianSelectionAfterAllyKilledJA, medicSelectionAfterAllyKilledJA;
    public AudioClip[] commanderSelectionAfterAllyKilledOrUncon, spartanSelectionAfterAllyKilledOrUncon, survivorSelectionAfterAllyKilledOrUncon, runnerSelectionAfterAllyKilledOrUncon, evaderSelectionAfterAllyKilledOrUncon, reservistSelectionAfterAllyKilledOrUncon, seekerSelectionAfterAllyKilledOrUncon, chameleonSelectionAfterAllyKilledOrUncon, scoutSelectionAfterAllyKilledOrUncon, infantrymanSelectionAfterAllyKilledOrUncon, operatorSelectionAfterAllyKilledOrUncon, earthquakeSelectionAfterAllyKilledOrUncon, hunterSelectionAfterAllyKilledOrUncon, cycloneSelectionAfterAllyKilledOrUncon, hammerSelectionAfterAllyKilledOrUncon, wolfSelectionAfterAllyKilledOrUncon, herculesSelectionAfterAllyKilledOrUncon, diplomatSelectionAfterAllyKilledOrUncon, technicianSelectionAfterAllyKilledOrUncon, medicSelectionAfterAllyKilledOrUncon;
    public AudioClip[] commanderSelectionAfterEnemyUseULF, spartanSelectionAfterEnemyUseULF, survivorSelectionAfterEnemyUseULF, runnerSelectionAfterEnemyUseULF, evaderSelectionAfterEnemyUseULF, reservistSelectionAfterEnemyUseULF, seekerSelectionAfterEnemyUseULF, chameleonSelectionAfterEnemyUseULF, scoutSelectionAfterEnemyUseULF, infantrymanSelectionAfterEnemyUseULF, operatorSelectionAfterEnemyUseULF, earthquakeSelectionAfterEnemyUseULF, hunterSelectionAfterEnemyUseULF, cycloneSelectionAfterEnemyUseULF, hammerSelectionAfterEnemyUseULF, wolfSelectionAfterEnemyUseULF, herculesSelectionAfterEnemyUseULF, diplomatSelectionAfterEnemyUseULF, technicianSelectionAfterEnemyUseULF, medicSelectionAfterEnemyUseULF;
    public AudioClip[] commanderSelectionAfterEnemyUseUHF, spartanSelectionAfterEnemyUseUHF, survivorSelectionAfterEnemyUseUHF, runnerSelectionAfterEnemyUseUHF, evaderSelectionAfterEnemyUseUHF, reservistSelectionAfterEnemyUseUHF, seekerSelectionAfterEnemyUseUHF, chameleonSelectionAfterEnemyUseUHF, scoutSelectionAfterEnemyUseUHF, infantrymanSelectionAfterEnemyUseUHF, operatorSelectionAfterEnemyUseUHF, earthquakeSelectionAfterEnemyUseUHF, hunterSelectionAfterEnemyUseUHF, cycloneSelectionAfterEnemyUseUHF, hammerSelectionAfterEnemyUseUHF, wolfSelectionAfterEnemyUseUHF, herculesSelectionAfterEnemyUseUHF, diplomatSelectionAfterEnemyUseUHF, technicianSelectionAfterEnemyUseUHF, medicSelectionAfterEnemyUseUHF;
    public AudioClip[] commanderSelectionAfterEnemyUseTabun, spartanSelectionAfterEnemyUseTabun, survivorSelectionAfterEnemyUseTabun, runnerSelectionAfterEnemyUseTabun, evaderSelectionAfterEnemyUseTabun, reservistSelectionAfterEnemyUseTabun, seekerSelectionAfterEnemyUseTabun, chameleonSelectionAfterEnemyUseTabun, scoutSelectionAfterEnemyUseTabun, infantrymanSelectionAfterEnemyUseTabun, operatorSelectionAfterEnemyUseTabun, earthquakeSelectionAfterEnemyUseTabun, hunterSelectionAfterEnemyUseTabun, cycloneSelectionAfterEnemyUseTabun, hammerSelectionAfterEnemyUseTabun, wolfSelectionAfterEnemyUseTabun, herculesSelectionAfterEnemyUseTabun, diplomatSelectionAfterEnemyUseTabun, technicianSelectionAfterEnemyUseTabun, medicSelectionAfterEnemyUseTabun;
    public AudioClip[] commanderSelectionAfterEnemyUseSmoke, spartanSelectionAfterEnemyUseSmoke, survivorSelectionAfterEnemyUseSmoke, runnerSelectionAfterEnemyUseSmoke, evaderSelectionAfterEnemyUseSmoke, reservistSelectionAfterEnemyUseSmoke, seekerSelectionAfterEnemyUseSmoke, chameleonSelectionAfterEnemyUseSmoke, scoutSelectionAfterEnemyUseSmoke, infantrymanSelectionAfterEnemyUseSmoke, operatorSelectionAfterEnemyUseSmoke, earthquakeSelectionAfterEnemyUseSmoke, hunterSelectionAfterEnemyUseSmoke, cycloneSelectionAfterEnemyUseSmoke, hammerSelectionAfterEnemyUseSmoke, wolfSelectionAfterEnemyUseSmoke, herculesSelectionAfterEnemyUseSmoke, diplomatSelectionAfterEnemyUseSmoke, technicianSelectionAfterEnemyUseSmoke, medicSelectionAfterEnemyUseSmoke;
    public AudioClip[] commanderSelectionAfterHeardSound, spartanSelectionAfterHeardSound, survivorSelectionAfterHeardSound, runnerSelectionAfterHeardSound, evaderSelectionAfterHeardSound, reservistSelectionAfterHeardSound, seekerSelectionAfterHeardSound, chameleonSelectionAfterHeardSound, scoutSelectionAfterHeardSound, infantrymanSelectionAfterHeardSound, operatorSelectionAfterHeardSound, earthquakeSelectionAfterHeardSound, hunterSelectionAfterHeardSound, cycloneSelectionAfterHeardSound, hammerSelectionAfterHeardSound, wolfSelectionAfterHeardSound, herculesSelectionAfterHeardSound, diplomatSelectionAfterHeardSound, technicianSelectionAfterHeardSound, medicSelectionAfterHeardSound;
    public AudioClip[] commanderSelectionAfterEnemyKilledJA, spartanSelectionAfterEnemyKilledJA, survivorSelectionAfterEnemyKilledJA, runnerSelectionAfterEnemyKilledJA, evaderSelectionAfterEnemyKilledJA, reservistSelectionAfterEnemyKilledJA, seekerSelectionAfterEnemyKilledJA, chameleonSelectionAfterEnemyKilledJA, scoutSelectionAfterEnemyKilledJA, infantrymanSelectionAfterEnemyKilledJA, operatorSelectionAfterEnemyKilledJA, earthquakeSelectionAfterEnemyKilledJA, hunterSelectionAfterEnemyKilledJA, cycloneSelectionAfterEnemyKilledJA, hammerSelectionAfterEnemyKilledJA, wolfSelectionAfterEnemyKilledJA, herculesSelectionAfterEnemyKilledJA, diplomatSelectionAfterEnemyKilledJA, technicianSelectionAfterEnemyKilledJA, medicSelectionAfterEnemyKilledJA;
    public AudioClip[] commanderSelectionAfterEnemyKilled, spartanSelectionAfterEnemyKilled, survivorSelectionAfterEnemyKilled, runnerSelectionAfterEnemyKilled, evaderSelectionAfterEnemyKilled, reservistSelectionAfterEnemyKilled, seekerSelectionAfterEnemyKilled, chameleonSelectionAfterEnemyKilled, scoutSelectionAfterEnemyKilled, infantrymanSelectionAfterEnemyKilled, operatorSelectionAfterEnemyKilled, earthquakeSelectionAfterEnemyKilled, hunterSelectionAfterEnemyKilled, cycloneSelectionAfterEnemyKilled, hammerSelectionAfterEnemyKilled, wolfSelectionAfterEnemyKilled, herculesSelectionAfterEnemyKilled, diplomatSelectionAfterEnemyKilled, technicianSelectionAfterEnemyKilled, medicSelectionAfterEnemyKilled;
    public AudioClip[] commanderSelectionAfterAllyMissesShot, spartanSelectionAfterAllyMissesShot, survivorSelectionAfterAllyMissesShot, runnerSelectionAfterAllyMissesShot, evaderSelectionAfterAllyMissesShot, reservistSelectionAfterAllyMissesShot, seekerSelectionAfterAllyMissesShot, chameleonSelectionAfterAllyMissesShot, scoutSelectionAfterAllyMissesShot, infantrymanSelectionAfterAllyMissesShot, operatorSelectionAfterAllyMissesShot, earthquakeSelectionAfterAllyMissesShot, hunterSelectionAfterAllyMissesShot, cycloneSelectionAfterAllyMissesShot, hammerSelectionAfterAllyMissesShot, wolfSelectionAfterAllyMissesShot, herculesSelectionAfterAllyMissesShot, diplomatSelectionAfterAllyMissesShot, technicianSelectionAfterAllyMissesShot, medicSelectionAfterAllyMissesShot;
    public AudioClip[] commanderSelectionAfterEnemyMissesShot, spartanSelectionAfterEnemyMissesShot, survivorSelectionAfterEnemyMissesShot, runnerSelectionAfterEnemyMissesShot, evaderSelectionAfterEnemyMissesShot, reservistSelectionAfterEnemyMissesShot, seekerSelectionAfterEnemyMissesShot, chameleonSelectionAfterEnemyMissesShot, scoutSelectionAfterEnemyMissesShot, infantrymanSelectionAfterEnemyMissesShot, operatorSelectionAfterEnemyMissesShot, earthquakeSelectionAfterEnemyMissesShot, hunterSelectionAfterEnemyMissesShot, cycloneSelectionAfterEnemyMissesShot, hammerSelectionAfterEnemyMissesShot, wolfSelectionAfterEnemyMissesShot, herculesSelectionAfterEnemyMissesShot, diplomatSelectionAfterEnemyMissesShot, technicianSelectionAfterEnemyMissesShot, medicSelectionAfterEnemyMissesShot;
    public AudioClip[] commanderSelectionAfterAllyUseULF, spartanSelectionAfterAllyUseULF, survivorSelectionAfterAllyUseULF, runnerSelectionAfterAllyUseULF, evaderSelectionAfterAllyUseULF, reservistSelectionAfterAllyUseULF, seekerSelectionAfterAllyUseULF, chameleonSelectionAfterAllyUseULF, scoutSelectionAfterAllyUseULF, infantrymanSelectionAfterAllyUseULF, operatorSelectionAfterAllyUseULF, earthquakeSelectionAfterAllyUseULF, hunterSelectionAfterAllyUseULF, cycloneSelectionAfterAllyUseULF, hammerSelectionAfterAllyUseULF, wolfSelectionAfterAllyUseULF, herculesSelectionAfterAllyUseULF, diplomatSelectionAfterAllyUseULF, technicianSelectionAfterAllyUseULF, medicSelectionAfterAllyUseULF;
    public AudioClip[] commanderSelectionAfterAllyUseUHF, spartanSelectionAfterAllyUseUHF, survivorSelectionAfterAllyUseUHF, runnerSelectionAfterAllyUseUHF, evaderSelectionAfterAllyUseUHF, reservistSelectionAfterAllyUseUHF, seekerSelectionAfterAllyUseUHF, chameleonSelectionAfterAllyUseUHF, scoutSelectionAfterAllyUseUHF, infantrymanSelectionAfterAllyUseUHF, operatorSelectionAfterAllyUseUHF, earthquakeSelectionAfterAllyUseUHF, hunterSelectionAfterAllyUseUHF, cycloneSelectionAfterAllyUseUHF, hammerSelectionAfterAllyUseUHF, wolfSelectionAfterAllyUseUHF, herculesSelectionAfterAllyUseUHF, diplomatSelectionAfterAllyUseUHF, technicianSelectionAfterAllyUseUHF, medicSelectionAfterAllyUseUHF;
    public AudioClip[] commanderSelectionWhileOneAmmo, spartanSelectionWhileOneAmmo, survivorSelectionWhileOneAmmo, runnerSelectionWhileOneAmmo, evaderSelectionWhileOneAmmo, reservistSelectionWhileOneAmmo, seekerSelectionWhileOneAmmo, chameleonSelectionWhileOneAmmo, scoutSelectionWhileOneAmmo, infantrymanSelectionWhileOneAmmo, operatorSelectionWhileOneAmmo, earthquakeSelectionWhileOneAmmo, hunterSelectionWhileOneAmmo, cycloneSelectionWhileOneAmmo, hammerSelectionWhileOneAmmo, wolfSelectionWhileOneAmmo, herculesSelectionWhileOneAmmo, diplomatSelectionWhileOneAmmo, technicianSelectionWhileOneAmmo, medicSelectionWhileOneAmmo;
    public AudioClip[] commanderSelectionWhileNoAmmo, spartanSelectionWhileNoAmmo, survivorSelectionWhileNoAmmo, runnerSelectionWhileNoAmmo, evaderSelectionWhileNoAmmo, reservistSelectionWhileNoAmmo, seekerSelectionWhileNoAmmo, chameleonSelectionWhileNoAmmo, scoutSelectionWhileNoAmmo, infantrymanSelectionWhileNoAmmo, operatorSelectionWhileNoAmmo, earthquakeSelectionWhileNoAmmo, hunterSelectionWhileNoAmmo, cycloneSelectionWhileNoAmmo, hammerSelectionWhileNoAmmo, wolfSelectionWhileNoAmmo, herculesSelectionWhileNoAmmo, diplomatSelectionWhileNoAmmo, technicianSelectionWhileNoAmmo, medicSelectionWhileNoAmmo;
    public AudioClip[] commanderSelectionWhileAllyBroken, spartanSelectionWhileAllyBroken, survivorSelectionWhileAllyBroken, runnerSelectionWhileAllyBroken, evaderSelectionWhileAllyBroken, reservistSelectionWhileAllyBroken, seekerSelectionWhileAllyBroken, chameleonSelectionWhileAllyBroken, scoutSelectionWhileAllyBroken, infantrymanSelectionWhileAllyBroken, operatorSelectionWhileAllyBroken, earthquakeSelectionWhileAllyBroken, hunterSelectionWhileAllyBroken, cycloneSelectionWhileAllyBroken, hammerSelectionWhileAllyBroken, wolfSelectionWhileAllyBroken, herculesSelectionWhileAllyBroken, diplomatSelectionWhileAllyBroken, technicianSelectionWhileAllyBroken, medicSelectionWhileAllyBroken;
    public AudioClip[] commanderSelectionWhilePoisoned, spartanSelectionWhilePoisoned, survivorSelectionWhilePoisoned, runnerSelectionWhilePoisoned, evaderSelectionWhilePoisoned, reservistSelectionWhilePoisoned, seekerSelectionWhilePoisoned, chameleonSelectionWhilePoisoned, scoutSelectionWhilePoisoned, infantrymanSelectionWhilePoisoned, operatorSelectionWhilePoisoned, earthquakeSelectionWhilePoisoned, hunterSelectionWhilePoisoned, cycloneSelectionWhilePoisoned, hammerSelectionWhilePoisoned, wolfSelectionWhilePoisoned, herculesSelectionWhilePoisoned, diplomatSelectionWhilePoisoned, technicianSelectionWhilePoisoned, medicSelectionWhilePoisoned;
    public AudioClip[] commanderSelectionWhileBroken, spartanSelectionWhileBroken, survivorSelectionWhileBroken, runnerSelectionWhileBroken, evaderSelectionWhileBroken, reservistSelectionWhileBroken, seekerSelectionWhileBroken, chameleonSelectionWhileBroken, scoutSelectionWhileBroken, infantrymanSelectionWhileBroken, operatorSelectionWhileBroken, earthquakeSelectionWhileBroken, hunterSelectionWhileBroken, cycloneSelectionWhileBroken, hammerSelectionWhileBroken, wolfSelectionWhileBroken, herculesSelectionWhileBroken, diplomatSelectionWhileBroken, technicianSelectionWhileBroken, medicSelectionWhileBroken;
    public AudioClip[] commanderSelectionWhileFrozen, spartanSelectionWhileFrozen, survivorSelectionWhileFrozen, runnerSelectionWhileFrozen, evaderSelectionWhileFrozen, reservistSelectionWhileFrozen, seekerSelectionWhileFrozen, chameleonSelectionWhileFrozen, scoutSelectionWhileFrozen, infantrymanSelectionWhileFrozen, operatorSelectionWhileFrozen, earthquakeSelectionWhileFrozen, hunterSelectionWhileFrozen, cycloneSelectionWhileFrozen, hammerSelectionWhileFrozen, wolfSelectionWhileFrozen, herculesSelectionWhileFrozen, diplomatSelectionWhileFrozen, technicianSelectionWhileFrozen, medicSelectionWhileFrozen;
    public AudioClip[] commanderSelectionWhileShaken, spartanSelectionWhileShaken, survivorSelectionWhileShaken, runnerSelectionWhileShaken, evaderSelectionWhileShaken, reservistSelectionWhileShaken, seekerSelectionWhileShaken, chameleonSelectionWhileShaken, scoutSelectionWhileShaken, infantrymanSelectionWhileShaken, operatorSelectionWhileShaken, earthquakeSelectionWhileShaken, hunterSelectionWhileShaken, cycloneSelectionWhileShaken, hammerSelectionWhileShaken, wolfSelectionWhileShaken, herculesSelectionWhileShaken, diplomatSelectionWhileShaken, technicianSelectionWhileShaken, medicSelectionWhileShaken;
    public AudioClip[] commanderSelectionWhileWavering, spartanSelectionWhileWavering, survivorSelectionWhileWavering, runnerSelectionWhileWavering, evaderSelectionWhileWavering, reservistSelectionWhileWavering, seekerSelectionWhileWavering, chameleonSelectionWhileWavering, scoutSelectionWhileWavering, infantrymanSelectionWhileWavering, operatorSelectionWhileWavering, earthquakeSelectionWhileWavering, hunterSelectionWhileWavering, cycloneSelectionWhileWavering, hammerSelectionWhileWavering, wolfSelectionWhileWavering, herculesSelectionWhileWavering, diplomatSelectionWhileWavering, technicianSelectionWhileWavering, medicSelectionWhileWavering;
    public AudioClip[] commanderSelectionWhileOneEnemyLeft, spartanSelectionWhileOneEnemyLeft, survivorSelectionWhileOneEnemyLeft, runnerSelectionWhileOneEnemyLeft, evaderSelectionWhileOneEnemyLeft, reservistSelectionWhileOneEnemyLeft, seekerSelectionWhileOneEnemyLeft, chameleonSelectionWhileOneEnemyLeft, scoutSelectionWhileOneEnemyLeft, infantrymanSelectionWhileOneEnemyLeft, operatorSelectionWhileOneEnemyLeft, earthquakeSelectionWhileOneEnemyLeft, hunterSelectionWhileOneEnemyLeft, cycloneSelectionWhileOneEnemyLeft, hammerSelectionWhileOneEnemyLeft, wolfSelectionWhileOneEnemyLeft, herculesSelectionWhileOneEnemyLeft, diplomatSelectionWhileOneEnemyLeft, technicianSelectionWhileOneEnemyLeft, medicSelectionWhileOneEnemyLeft;
    public AudioClip[] commanderSelectionWhileTwoEnemyLeft, spartanSelectionWhileTwoEnemyLeft, survivorSelectionWhileTwoEnemyLeft, runnerSelectionWhileTwoEnemyLeft, evaderSelectionWhileTwoEnemyLeft, reservistSelectionWhileTwoEnemyLeft, seekerSelectionWhileTwoEnemyLeft, chameleonSelectionWhileTwoEnemyLeft, scoutSelectionWhileTwoEnemyLeft, infantrymanSelectionWhileTwoEnemyLeft, operatorSelectionWhileTwoEnemyLeft, earthquakeSelectionWhileTwoEnemyLeft, hunterSelectionWhileTwoEnemyLeft, cycloneSelectionWhileTwoEnemyLeft, hammerSelectionWhileTwoEnemyLeft, wolfSelectionWhileTwoEnemyLeft, herculesSelectionWhileTwoEnemyLeft, diplomatSelectionWhileTwoEnemyLeft, technicianSelectionWhileTwoEnemyLeft, medicSelectionWhileTwoEnemyLeft;
    public AudioClip[] commanderSelectionWhileThreeEnemyLeft, spartanSelectionWhileThreeEnemyLeft, survivorSelectionWhileThreeEnemyLeft, runnerSelectionWhileThreeEnemyLeft, evaderSelectionWhileThreeEnemyLeft, reservistSelectionWhileThreeEnemyLeft, seekerSelectionWhileThreeEnemyLeft, chameleonSelectionWhileThreeEnemyLeft, scoutSelectionWhileThreeEnemyLeft, infantrymanSelectionWhileThreeEnemyLeft, operatorSelectionWhileThreeEnemyLeft, earthquakeSelectionWhileThreeEnemyLeft, hunterSelectionWhileThreeEnemyLeft, cycloneSelectionWhileThreeEnemyLeft, hammerSelectionWhileThreeEnemyLeft, wolfSelectionWhileThreeEnemyLeft, herculesSelectionWhileThreeEnemyLeft, diplomatSelectionWhileThreeEnemyLeft, technicianSelectionWhileThreeEnemyLeft, medicSelectionWhileThreeEnemyLeft;
    public AudioClip[] commanderSelectionWhileKdNegThreeOrLess, spartanSelectionWhileKdNegThreeOrLess, survivorSelectionWhileKdNegThreeOrLess, runnerSelectionWhileKdNegThreeOrLess, evaderSelectionWhileKdNegThreeOrLess, reservistSelectionWhileKdNegThreeOrLess, seekerSelectionWhileKdNegThreeOrLess, chameleonSelectionWhileKdNegThreeOrLess, scoutSelectionWhileKdNegThreeOrLess, infantrymanSelectionWhileKdNegThreeOrLess, operatorSelectionWhileKdNegThreeOrLess, earthquakeSelectionWhileKdNegThreeOrLess, hunterSelectionWhileKdNegThreeOrLess, cycloneSelectionWhileKdNegThreeOrLess, hammerSelectionWhileKdNegThreeOrLess, wolfSelectionWhileKdNegThreeOrLess, herculesSelectionWhileKdNegThreeOrLess, diplomatSelectionWhileKdNegThreeOrLess, technicianSelectionWhileKdNegThreeOrLess, medicSelectionWhileKdNegThreeOrLess;
    public AudioClip[] commanderSelectionWhileHPFourOrLess, spartanSelectionWhileHPFourOrLess, survivorSelectionWhileHPFourOrLess, runnerSelectionWhileHPFourOrLess, evaderSelectionWhileHPFourOrLess, reservistSelectionWhileHPFourOrLess, seekerSelectionWhileHPFourOrLess, chameleonSelectionWhileHPFourOrLess, scoutSelectionWhileHPFourOrLess, infantrymanSelectionWhileHPFourOrLess, operatorSelectionWhileHPFourOrLess, earthquakeSelectionWhileHPFourOrLess, hunterSelectionWhileHPFourOrLess, cycloneSelectionWhileHPFourOrLess, hammerSelectionWhileHPFourOrLess, wolfSelectionWhileHPFourOrLess, herculesSelectionWhileHPFourOrLess, diplomatSelectionWhileHPFourOrLess, technicianSelectionWhileHPFourOrLess, medicSelectionWhileHPFourOrLess;
    public AudioClip[] commanderSelectionGeneric, spartanSelectionGeneric, survivorSelectionGeneric, runnerSelectionGeneric, evaderSelectionGeneric, reservistSelectionGeneric, seekerSelectionGeneric, chameleonSelectionGeneric, scoutSelectionGeneric, infantrymanSelectionGeneric, operatorSelectionGeneric, earthquakeSelectionGeneric, hunterSelectionGeneric, cycloneSelectionGeneric, hammerSelectionGeneric, wolfSelectionGeneric, herculesSelectionGeneric, diplomatSelectionGeneric, technicianSelectionGeneric, medicSelectionGeneric;

    //basic sound functions
    public void PlaySound(AudioClip clip)
    {
        if (!isMute)
        {
            // Check if the sound is already playing
            if (!playingSounds.ContainsKey(clip))
            {
                // Start the coroutine to play the sound and track it
                Coroutine soundCoroutine = StartCoroutine(PlayAndTrackSound(clip));
                playingSounds.Add(clip, soundCoroutine);
            }
        }
    }
    private IEnumerator PlayAndTrackSound(AudioClip clip)
    {
        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Remove the sound from the tracking dictionary after it finishes
        playingSounds.Remove(clip);
    }







    //meta sfx functions
    public void PlayBanzai()
    {
        if (!banzaiPlayed)
        {
            PlaySound(banzai);
            banzaiPlayed = true;
        }
    }
    public void PlayOverrideAlarm()
    {
        PlaySound(overrideAlarm);
    }
    public void PlayDetectionAlarm()
    {
        PlaySound(detectionAlarm);
    }
    public void PlayButtonPress()
    {
        PlaySound(buttonPress);
    }
    public void PlayPromotion()
    {
        PlaySound(levelUp);
    }
    public void PlayOvermoveAlarm()
    {
        PlaySound(overmoveAlarm);
    }
    public void PlayGameOverMusic()
    {
        PlaySound(gameOverMusic);
    }
    public void PlayFailedUpgrade()
    {
        PlaySound(failAbilityUpgrade);
    }
    public void PlaySucceededUpgrade()
    {
        PlaySound(succeedAbilityUpgrade);
    }
    public void PlayNewAbility()
    {
        PlaySound(newAbilityUpgrade);
    }





    //game sfx functions
    public void PlayExplosion()
    {
        PlaySound(explosion);
    }
    public void PlayMeleeResolution(string result)
    {
        if (result.Equals("counter"))
            PlaySound(meleeCounter);
        else if (result.Equals("breakeven"))
            PlaySound(meleeBreakeven);
        else if (result.Equals("successStatic"))
            PlaySound(meleeSuccessStatic);
        else if (result.Equals("successCharge"))
            PlaySound(meleeSuccessCharge);
    }
    public void PlayPlaceClaymore()
    {
        PlaySound(placeClaymore);
    }
    public void PlayDipelecResolution(string result)
    {
        if (result.Contains("Hack"))
            PlaySound(dipelecHacking);
        else if (result.Contains("Negotiation"))
            PlaySound(dipelecNegotiating);

        if (result.Contains("Fail"))
            PlaySound(dipelecFail);
        else if (result.Contains("Success"))
        {
            if (result.Contains("L1"))
                PlaySound(dipelecSuccessL1);
            else if (result.Contains("L2"))
                PlaySound(dipelecSuccessL2);
            else if (result.Contains("L3"))
                PlaySound(dipelecSuccessL3);
            else if (result.Contains("L4"))
                PlaySound(dipelecSuccessL4);
        }
    }
    public void PlayShotResolution(Item gun)
    {
        if (gun.IsAssaultRifle())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotAR);
        }
        else if (gun.IsLMG())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotLMG);
        }
        else if (gun.IsPistol())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotPi);
        }
        else if (gun.IsRifle())
        {
            PlaySound(shotRi);
        }
        else if (gun.IsShotgun())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotSh);
        }
        else if (gun.IsSMG())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotSMG);
        }
        else if (gun.IsSniper())
        {
            if (gun.HasSuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotSn);
        }
    }
    public void PlaySuppressionResolution(Item gun)
    {
        if (gun.IsLMG() || gun.IsSMG() || gun.IsAssaultRifle())
            PlaySound(shotSuppressLMG_SMG_AR);
        else if (gun.IsPistol() || gun.IsRifle() || gun.IsSniper() || gun.IsShotgun())
            PlaySound(shotSuppressPi_Ri_Sn_Sh);
    }
    public void PlayReload(Item gun)
    {
        if (gun.IsAssaultRifle())
            PlaySound(reloadAR);
        else if (gun.IsLMG())
            PlaySound(reloadLMG);
        else if (gun.IsPistol())
            PlaySound(reloadPi);
        else if (gun.IsRifle())
            PlaySound(reloadRi);
        else if (gun.IsShotgun())
            PlaySound(reloadSh);
        else if (gun.IsSMG())
            PlaySound(reloadSMG);
        else if (gun.IsSniper())
            PlaySound(reloadSn);
    }
    public void PlayCoverDestruction()
    {
        PlaySound(coverDestruction);
    }
    public void PlayFallFromHeight()
    {
        PlaySound(fallFromHeight);
    }
    public void PlayStructuralCollapse()
    {
        PlaySound(structuralCollapse);
    }
    public void PlayEquipArmour()
    {
        PlaySound(equipArmour);
    }
    public void PlayEquipWearableGear()
    {
        PlaySound(equipWearableGear);
    }
    public void PlayConfigGeneral()
    {
        PlaySound(configureGeneral);
    }
    public void PlayUseGrenade(Item grenade)
    {
        if (grenade.IsFrag())
            PlaySound(itemUseFrag);
        else if (grenade.IsFlashbang())
            PlaySound(itemUseFlash);
        else if (grenade.IsSmoke())
            PlaySound(itemUseSmoke);
        else if (grenade.IsTabun())
            PlaySound(itemUseTabun);
    }
    public void PlayUseDepBeacon()
    {
        PlaySound(itemUseDepBeacon);
    }
    public void PlayUseETool()
    {
        PlaySound(itemUseETool);
    }
    public void PlayUseFood()
    {
        PlaySound(itemUseFood);
    }
    public void PlayUseMedikit()
    {
        PlaySound(itemUseMedikit);
    }
    public void PlayUsePoisonSatchel()
    {
        PlaySound(itemUsePoisonSatchel);
    }
    public void PlayUseSyringe()
    {
        PlaySound(itemUseSyringe);
    }
    public void PlayUseWater()
    {
        PlaySound(itemUseWater);
    }
    public void PlayUsePoisonedItem()
    {
        PlaySound(itemUsePoisonedItem);
    }









    //s voices
    public void PlayVoice(Soldier s, AudioClip clip)
    {
        if (!isMute)
        {
            // Check if the sound is already playing
            if (!playingSounds.ContainsKey(clip))
            {
                if (!s.isSpeaking) //block if s is already speaking
                {
                    s.isSpeaking = true;

                    // Start the coroutine to play the sound and track it
                    Coroutine soundCoroutine = StartCoroutine(PlayAndTrackVoice(s, clip));
                    playingSounds.Add(clip, soundCoroutine);
                }
            }
        }
    }
    private IEnumerator PlayAndTrackVoice(Soldier s, AudioClip clip)
    {
        // Play the sound using PlayOneShot
        audioSource.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Remove the sound from the tracking dictionary after it finishes
        playingSounds.Remove(clip);
        s.isSpeaking = false;
    }

    public void PlayRandomVoice(Soldier s, AudioClip[] audioClipArray)
    {
        if (audioClipArray.Any())
        {
            if (game.CoinFlip()) //50% chance of silence
            {
                PlayVoice(s, audioClipArray[HelperFunctions.RandomNumber(0, audioClipArray.Length - 1)]);
            }
        }
    }
    public void PlaySoldierConfigNearGB(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderConfigNearGB);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanConfigNearGB);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorConfigNearGB);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerConfigNearGB);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderConfigNearGB);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistConfigNearGB);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerConfigNearGB);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerConfigNearGB);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutConfigNearGB);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanConfigNearGB);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorConfigNearGB);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeConfigNearGB);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterConfigNearGB);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneConfigNearGB);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerConfigNearGB);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfConfigNearGB);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesConfigNearGB);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatConfigNearGB);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianConfigNearGB);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicConfigNearGB);
    }
    public void PlaySoldierConfirmMove(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderConfirmMove);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanConfirmMove);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorConfirmMove);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerConfirmMove);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderConfirmMove);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistConfirmMove);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerConfirmMove);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerConfirmMove);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutConfirmMove);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanConfirmMove);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorConfirmMove);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeConfirmMove);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterConfirmMove);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneConfirmMove);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerConfirmMove);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfConfirmMove);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesConfirmMove);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatConfirmMove);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianConfirmMove);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicConfirmMove);
    }
    public void PlaySoldierDetectClaymore(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderDetectClaymore);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanDetectClaymore);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorDetectClaymore);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerDetectClaymore);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderDetectClaymore);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistDetectClaymore);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerDetectClaymore);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerDetectClaymore);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutDetectClaymore);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanDetectClaymore);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorDetectClaymore);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeDetectClaymore);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterDetectClaymore);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneDetectClaymore);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerDetectClaymore);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfDetectClaymore);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesDetectClaymore);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatDetectClaymore);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianDetectClaymore);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicDetectClaymore);
    }
    public void PlaySoldierEnterOverwatch(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianEnterOverwatch);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicEnterOverwatch);
    }
    public void PlaySoldierHealAlly(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderHealAlly);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanHealAlly);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorHealAlly);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerHealAlly);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderHealAlly);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistHealAlly);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerHealAlly);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerHealAlly);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutHealAlly);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanHealAlly);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorHealAlly);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeHealAlly);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterHealAlly);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneHealAlly);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerHealAlly);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfHealAlly);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesHealAlly);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatHealAlly);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianHealAlly);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicHealAlly);
    }
    public void PlaySoldierEquipArmour(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderEquipArmour);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanEquipArmour);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorEquipArmour);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerEquipArmour);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderEquipArmour);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistEquipArmour);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerEquipArmour);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerEquipArmour);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutEquipArmour);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanEquipArmour);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorEquipArmour);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeEquipArmour);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterEquipArmour);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneEquipArmour);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerEquipArmour);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfEquipArmour);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesEquipArmour);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatEquipArmour);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianEquipArmour);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicEquipArmour);
    }
    public void PlaySoldierPlaceClaymore(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakePlaceClaymore);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cyclonePlaceClaymore);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianPlaceClaymore);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicPlaceClaymore);
    }
    public void PlaySoldierUseGrenade(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderUseGrenade);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanUseGrenade);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorUseGrenade);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerUseGrenade);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderUseGrenade);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistUseGrenade);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerUseGrenade);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerUseGrenade);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutUseGrenade);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanUseGrenade);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorUseGrenade);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeUseGrenade);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterUseGrenade);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneUseGrenade);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerUseGrenade);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfUseGrenade);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesUseGrenade);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatUseGrenade);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianUseGrenade);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicUseGrenade);
    }
    public void PlaySoldierUseTabun(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderUseTabun);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanUseTabun);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorUseTabun);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerUseTabun);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderUseTabun);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistUseTabun);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerUseTabun);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerUseTabun);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutUseTabun);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanUseTabun);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorUseTabun);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeUseTabun);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterUseTabun);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneUseTabun);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerUseTabun);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfUseTabun);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesUseTabun);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatUseTabun);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianUseTabun);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicUseTabun);
    }
    public void PlaySoldierUseSmoke(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderUseSmoke);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanUseSmoke);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorUseSmoke);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerUseSmoke);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderUseSmoke);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistUseSmoke);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerUseSmoke);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerUseSmoke);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutUseSmoke);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanUseSmoke);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorUseSmoke);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeUseSmoke);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterUseSmoke);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneUseSmoke);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerUseSmoke);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfUseSmoke);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesUseSmoke);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatUseSmoke);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianUseSmoke);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicUseSmoke);
    }
    public void PlaySoldierKillEnemy(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderKillEnemy);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanKillEnemy);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorKillEnemy);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerKillEnemy);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderKillEnemy);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistKillEnemy);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerKillEnemy);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerKillEnemy);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutKillEnemy);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanKillEnemy);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorKillEnemy);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeKillEnemy);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterKillEnemy);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneKillEnemy);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerKillEnemy);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfKillEnemy);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesKillEnemy);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatKillEnemy);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianKillEnemy);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicKillEnemy);
    }
    public void PlaySoldierMeleeBreakeven(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianMeleeBreakeven);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicMeleeBreakeven);
    }
    public void PlaySoldierMeleeMove(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderMeleeMove);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanMeleeMove);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorMeleeMove);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerMeleeMove);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderMeleeMove);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistMeleeMove);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerMeleeMove);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerMeleeMove);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutMeleeMove);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanMeleeMove);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorMeleeMove);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeMeleeMove);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterMeleeMove);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneMeleeMove);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerMeleeMove);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfMeleeMove);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesMeleeMove);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatMeleeMove);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianMeleeMove);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicMeleeMove);
    }
    public void PlaySoldierShotMiss(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderShotMiss);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanShotMiss);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorShotMiss);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerShotMiss);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderShotMiss);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistShotMiss);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerShotMiss);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerShotMiss);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutShotMiss);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanShotMiss);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorShotMiss);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeShotMiss);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterShotMiss);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneShotMiss);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerShotMiss);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfShotMiss);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesShotMiss);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatShotMiss);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianShotMiss);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicShotMiss);
    }
    public void PlaySoldierPickupUHF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderPickupUHF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanPickupUHF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorPickupUHF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerPickupUHF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderPickupUHF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistPickupUHF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerPickupUHF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerPickupUHF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutPickupUHF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanPickupUHF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorPickupUHF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakePickupUHF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterPickupUHF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cyclonePickupUHF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerPickupUHF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfPickupUHF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesPickupUHF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatPickupUHF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianPickupUHF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicPickupUHF);
    }
    public void PlaySoldierPickupULF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderPickupULF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanPickupULF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorPickupULF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerPickupULF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderPickupULF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistPickupULF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerPickupULF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerPickupULF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutPickupULF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanPickupULF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorPickupULF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakePickupULF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterPickupULF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cyclonePickupULF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerPickupULF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfPickupULF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesPickupULF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatPickupULF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianPickupULF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicPickupULF);
    }
    public void PlaySoldierSeeEnemy(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSeeEnemy);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSeeEnemy);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSeeEnemy);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSeeEnemy);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSeeEnemy);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSeeEnemy);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSeeEnemy);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSeeEnemy);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSeeEnemy);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSeeEnemy);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSeeEnemy);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSeeEnemy);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSeeEnemy);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSeeEnemy);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSeeEnemy);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSeeEnemy);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSeeEnemy);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSeeEnemy);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSeeEnemy);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSeeEnemy);
    }
    public void PlaySoldierSuppressEnemy(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSuppressEnemy);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSuppressEnemy);
    }
    public void PlayUHFDialUp()
    {
        PlaySound(uhfDialUp[HelperFunctions.RandomNumber(0, uhfDialUp.Length - 1)]);
    }
    public void PlayUHFResult(int roll)
    {
        if (roll == 6)
            PlaySound(uhfDirectHit[HelperFunctions.RandomNumber(0, uhfDirectHit.Length - 1)]);
        else if (roll == 1)
            PlaySound(uhfMiss[HelperFunctions.RandomNumber(0, uhfMiss.Length - 1)]);
        else
            PlaySound(uhfHit[HelperFunctions.RandomNumber(0, uhfHit.Length - 1)]);
    }
    public void PlayULFResult(string result)
    {
        if (result.Contains("Jamming") || result.Contains("Spying"))
            PlaySound(ulfSuccess[HelperFunctions.RandomNumber(0, ulfSuccess.Length - 1)]);
        else
            PlaySound(ulfFail[HelperFunctions.RandomNumber(0, ulfFail.Length - 1)]);
    }







    //s selection dialogue
    public void UnsetAllSoldierSelectionSoundFlags(Soldier s)
    {
        soldierTakenDamage.Remove(s.Id);
        soldierAllyKilledJA.Remove(s.Id);
        soldierAllyKilledOrUncon.Remove(s.Id);
        soldierEnemyUseULF.Remove(s.Id);
        soldierEnemyUseUHF.Remove(s.Id);
        soldierEnemyUseTabun.Remove(s.Id);
        soldierEnemyUseSmoke.Remove(s.Id);
        soldierHeardSound.Remove(s.Id);
        soldierEnemyKilledJA.Remove(s.Id);
        soldierEnemyKilled.Remove(s.Id);
        soldierAllyMissesShot.Remove(s.Id);
        soldierEnemyMissesShot.Remove(s.Id);
        soldierAllyUseULF.Remove(s.Id);
        soldierAllyUseUHF.Remove(s.Id);
    }
    public void SetSoundFlag(Soldier s, List<string> soldierList)
    {
        if (s.IsConscious())
        {
            if (!soldierList.Contains(s.Id))
                soldierList.Add(s.Id);
        }
    }
    public void SetSoldierSelectionSoundFlagAfterDamage(Soldier s)
    {
        SetSoundFlag(s, soldierTakenDamage);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledJA(Soldier s)
    {
        SetSoundFlag(s, soldierAllyKilledJA);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledOrUncon(Soldier s)
    {
        SetSoundFlag(s, soldierAllyKilledOrUncon);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseULF(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyUseULF);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseUHF(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyUseUHF);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseTabun(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyUseTabun);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseSmoke(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyUseSmoke);
    }
    public void SetSoldierSelectionSoundFlagAfterHeardSound(Soldier s)
    {
        SetSoundFlag(s, soldierHeardSound);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyKilledJA(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyKilledJA);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyKilled(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyKilled);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyMissesShot(Soldier s)
    {
        SetSoundFlag(s, soldierAllyMissesShot);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyMissesShot(Soldier s)
    {
        SetSoundFlag(s, soldierEnemyMissesShot);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyUseULF(Soldier s)
    {
        SetSoundFlag(s, soldierAllyUseULF);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyUseUHF(Soldier s)
    {
        SetSoundFlag(s, soldierAllyUseUHF);
    }
    public void PlaySoldierSelection(Soldier s)
    {
        if (s.IsConscious())
        {
            if (soldierTakenDamage.Contains(s.Id))
                PlaySoldierSelectionAfterDamage(s);
            else if (soldierAllyKilledJA.Contains(s.Id))
                PlaySoldierSelectionAfterAllyKilledJA(s);
            else if (soldierAllyKilledOrUncon.Contains(s.Id))
                PlaySoldierSelectionAfterAllyKilledOrUncon(s);
            else if (soldierEnemyUseULF.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyUseULF(s);
            else if (soldierEnemyUseUHF.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyUseUHF(s);
            else if (soldierEnemyUseTabun.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyUseTabun(s);
            else if (soldierEnemyUseSmoke.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyUseSmoke(s);
            else if (soldierHeardSound.Contains(s.Id))
                PlaySoldierSelectionAfterHeardSound(s);
            else if (soldierEnemyKilledJA.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyKilledJA(s);
            else if (soldierEnemyKilled.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyKilled(s);
            else if (soldierAllyMissesShot.Contains(s.Id))
                PlaySoldierSelectionAfterAllyMissesShot(s);
            else if (soldierEnemyMissesShot.Contains(s.Id))
                PlaySoldierSelectionAfterEnemyMissesShot(s);
            else if (soldierAllyUseULF.Contains(s.Id))
                PlaySoldierSelectionAfterAllyUseULF(s);
            else if (soldierAllyUseUHF.Contains(s.Id))
                PlaySoldierSelectionAfterAllyUseUHF(s);
            else if (s.HasGunsEquipped() && s.HasOneAmmo())
                PlaySoldierSelectionWhileOneAmmo(s);
            else if (s.HasGunsEquipped() && s.HasNoAmmo())
                PlaySoldierSelectionWhileNoAmmo(s);
            else if (s.HasBrokenAllies())
                PlaySoldierSelectionWhileAllyBroken(s);
            else if (s.IsPoisoned())
                PlaySoldierSelectionWhilePoisoned(s);
            else if (s.IsBroken())
                PlaySoldierSelectionWhileBroken(s);
            else if (s.IsFrozen())
                PlaySoldierSelectionWhileFrozen(s);
            else if (s.IsShaken())
                PlaySoldierSelectionWhileShaken(s);
            else if (s.IsWavering())
                PlaySoldierSelectionWhileWavering(s);
            else if (OneEnemyLeft(s))
                PlaySoldierSelectionWhileOneEnemyLeft(s);
            else if (TwoEnemyLeft(s))
                PlaySoldierSelectionWhileTwoEnemyLeft(s);
            else if (ThreeEnemyLeft(s))
                PlaySoldierSelectionWhileThreeEnemyLeft(s);
            else if (s.GetKd() <= -3)
                PlaySoldierSelectionWhileKdNegThreeOrLess(s);
            else if (s.hp <= 4)
                PlaySoldierSelectionWhileHPFourOrLess(s);
            else
                PlaySoldierSelectionGeneric(s);
        }

        //unset other flags
        UnsetAllSoldierSelectionSoundFlags(s);
    }
    public void PlaySoldierSelectionAfterDamage(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterDamage);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterDamage);
    }
    public void PlaySoldierSelectionAfterAllyKilledJA(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterAllyKilledJA);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterAllyKilledJA);
    }
    public void PlaySoldierSelectionAfterAllyKilledOrUncon(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterAllyKilledOrUncon);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterAllyKilledOrUncon);
    }
    public void PlaySoldierSelectionAfterEnemyUseULF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyUseULF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyUseULF);
    }
    public void PlaySoldierSelectionAfterEnemyUseUHF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyUseUHF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyUseUHF);
    }
    public void PlaySoldierSelectionAfterEnemyUseTabun(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyUseTabun);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyUseTabun);
    }
    public void PlaySoldierSelectionAfterEnemyUseSmoke(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyUseSmoke);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyUseSmoke);
    }
    public void PlaySoldierSelectionAfterHeardSound(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterHeardSound);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterHeardSound);
    }
    public void PlaySoldierSelectionAfterEnemyKilledJA(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyKilledJA);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyKilledJA);
    }
    public void PlaySoldierSelectionAfterEnemyKilled(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyKilled);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyKilled);
    }
    public void PlaySoldierSelectionAfterAllyMissesShot(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterAllyMissesShot);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterAllyMissesShot);
    }
    public void PlaySoldierSelectionAfterEnemyMissesShot(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterEnemyMissesShot);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterEnemyMissesShot);
    }
    public void PlaySoldierSelectionAfterAllyUseULF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterAllyUseULF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterAllyUseULF);
    }
    public void PlaySoldierSelectionAfterAllyUseUHF(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionAfterAllyUseUHF);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionAfterAllyUseUHF);
    }
    public void PlaySoldierSelectionWhileOneAmmo(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileOneAmmo);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileOneAmmo);
    }
    public void PlaySoldierSelectionWhileNoAmmo(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileNoAmmo);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileNoAmmo);
    }
    public void PlaySoldierSelectionWhileAllyBroken(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileAllyBroken);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileAllyBroken);
    }
    public void PlaySoldierSelectionWhilePoisoned(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhilePoisoned);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhilePoisoned);
    }
    public void PlaySoldierSelectionWhileBroken(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileBroken);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileBroken);
    }
    public void PlaySoldierSelectionWhileFrozen(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileFrozen);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileFrozen);
    }
    public void PlaySoldierSelectionWhileShaken(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileShaken);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileShaken);
    }
    public void PlaySoldierSelectionWhileWavering(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileWavering);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileWavering);
    }
    public void PlaySoldierSelectionWhileOneEnemyLeft(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileOneEnemyLeft);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileOneEnemyLeft);
    }
    public void PlaySoldierSelectionWhileTwoEnemyLeft(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileTwoEnemyLeft);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileTwoEnemyLeft);
    }
    public void PlaySoldierSelectionWhileThreeEnemyLeft(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileThreeEnemyLeft);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileThreeEnemyLeft);
    }
    public void PlaySoldierSelectionWhileKdNegThreeOrLess(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileKdNegThreeOrLess);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileKdNegThreeOrLess);
    }
    public void PlaySoldierSelectionWhileHPFourOrLess(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionWhileHPFourOrLess);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionWhileHPFourOrLess);
    }
    public void PlaySoldierSelectionGeneric(Soldier s)
    {
        if (s.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(s, commanderSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(s, spartanSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(s, survivorSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(s, runnerSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(s, evaderSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(s, reservistSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(s, seekerSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(s, seekerSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(s, scoutSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(s, infantrymanSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(s, operatorSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(s, earthquakeSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(s, hunterSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(s, cycloneSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(s, hammerSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(s, wolfSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(s, herculesSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(s, diplomatSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(s, technicianSelectionGeneric);
        else if (s.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(s, medicSelectionGeneric);
    }
    public bool OneEnemyLeft(Soldier s)
    {
        if (EnemyCount(s) == 1)
            return true;
        return false;
    }
    public bool TwoEnemyLeft(Soldier s)
    {
        if (EnemyCount(s) == 2)
            return true;
        return false;
    }
    public bool ThreeEnemyLeft(Soldier s)
    {
        if (EnemyCount(s) == 3)
            return true;
        return false;
    }
    public int EnemyCount(Soldier s) 
    {
        int enemyCount = 0;
        foreach (Soldier s1 in game.AllSoldiers())
        {
            if (s.IsOppositeTeamAs(s1) && !s1.IsDead())
                enemyCount++;
        }
        return enemyCount;
    }
}
