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
        print($"Playing sound \"{clip.name}\"");
        audioSource.PlayOneShot(clip);

        // Wait for the clip to finish
        yield return new WaitForSeconds(clip.length);

        // Remove the sound from the tracking dictionary after it finishes
        print($"Closing sound \"{clip.name}\"");
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
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotAR);
        }
        else if (gun.IsLMG())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotLMG);
        }
        else if (gun.IsPistol())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotPi);
        }
        else if (gun.IsRifle())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotRi);
        }
        else if (gun.IsShotgun())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedPi_Ri_Sn_Sh);
            else
                PlaySound(shotSh);
        }
        else if (gun.IsSMG())
        {
            if (gun.SuppressorAttached())
                PlaySound(shotSilencedLMG_SMG_AR);
            else
                PlaySound(shotSMG);
        }
        else if (gun.IsSniper())
        {
            if (gun.SuppressorAttached())
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









    //soldier voices
    public void PlayRandomVoice(AudioClip[] audioClipArray)
    {
        if (audioClipArray.Any())
        {
            //if (game.CoinFlip()) //50% chance of silence
            //{
                PlaySound(audioClipArray[HelperFunctions.RandomNumber(0, audioClipArray.Length - 1)]);
            //}
        }
    }
    public void PlaySoldierConfigNearGB(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderConfigNearGB);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanConfigNearGB);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorConfigNearGB);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerConfigNearGB);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderConfigNearGB);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistConfigNearGB);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerConfigNearGB);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerConfigNearGB);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutConfigNearGB);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanConfigNearGB);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorConfigNearGB);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeConfigNearGB);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterConfigNearGB);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneConfigNearGB);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerConfigNearGB);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfConfigNearGB);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesConfigNearGB);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatConfigNearGB);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianConfigNearGB);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicConfigNearGB);
    }
    public void PlaySoldierConfirmMove(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderConfirmMove);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanConfirmMove);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorConfirmMove);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerConfirmMove);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderConfirmMove);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistConfirmMove);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerConfirmMove);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerConfirmMove);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutConfirmMove);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanConfirmMove);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorConfirmMove);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeConfirmMove);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterConfirmMove);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneConfirmMove);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerConfirmMove);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfConfirmMove);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesConfirmMove);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatConfirmMove);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianConfirmMove);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicConfirmMove);
    }
    public void PlaySoldierDetectClaymore(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderDetectClaymore);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanDetectClaymore);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorDetectClaymore);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerDetectClaymore);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderDetectClaymore);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistDetectClaymore);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerDetectClaymore);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerDetectClaymore);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutDetectClaymore);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanDetectClaymore);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorDetectClaymore);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeDetectClaymore);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterDetectClaymore);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneDetectClaymore);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerDetectClaymore);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfDetectClaymore);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesDetectClaymore);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatDetectClaymore);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianDetectClaymore);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicDetectClaymore);
    }
    public void PlaySoldierEnterOverwatch(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderEnterOverwatch);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanEnterOverwatch);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorEnterOverwatch);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerEnterOverwatch);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderEnterOverwatch);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistEnterOverwatch);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerEnterOverwatch);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerEnterOverwatch);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutEnterOverwatch);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanEnterOverwatch);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorEnterOverwatch);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeEnterOverwatch);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterEnterOverwatch);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneEnterOverwatch);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerEnterOverwatch);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfEnterOverwatch);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesEnterOverwatch);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatEnterOverwatch);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianEnterOverwatch);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicEnterOverwatch);
    }
    public void PlaySoldierHealAlly(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderHealAlly);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanHealAlly);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorHealAlly);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerHealAlly);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderHealAlly);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistHealAlly);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerHealAlly);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerHealAlly);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutHealAlly);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanHealAlly);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorHealAlly);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeHealAlly);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterHealAlly);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneHealAlly);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerHealAlly);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfHealAlly);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesHealAlly);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatHealAlly);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianHealAlly);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicHealAlly);
    }
    public void PlaySoldierEquipArmour(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderEquipArmour);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanEquipArmour);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorEquipArmour);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerEquipArmour);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderEquipArmour);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistEquipArmour);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerEquipArmour);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerEquipArmour);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutEquipArmour);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanEquipArmour);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorEquipArmour);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeEquipArmour);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterEquipArmour);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneEquipArmour);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerEquipArmour);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfEquipArmour);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesEquipArmour);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatEquipArmour);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianEquipArmour);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicEquipArmour);
    }
    public void PlaySoldierPlaceClaymore(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPlaceClaymore);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPlaceClaymore);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPlaceClaymore);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPlaceClaymore);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPlaceClaymore);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPlaceClaymore);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPlaceClaymore);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPlaceClaymore);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPlaceClaymore);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPlaceClaymore);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPlaceClaymore);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePlaceClaymore);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPlaceClaymore);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePlaceClaymore);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPlaceClaymore);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPlaceClaymore);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPlaceClaymore);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPlaceClaymore);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPlaceClaymore);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPlaceClaymore);
    }
    public void PlaySoldierUseGrenade(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseGrenade);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseGrenade);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseGrenade);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseGrenade);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseGrenade);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseGrenade);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseGrenade);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseGrenade);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseGrenade);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseGrenade);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseGrenade);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseGrenade);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseGrenade);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseGrenade);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseGrenade);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseGrenade);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseGrenade);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseGrenade);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseGrenade);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseGrenade);
    }
    public void PlaySoldierUseTabun(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseTabun);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseTabun);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseTabun);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseTabun);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseTabun);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseTabun);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseTabun);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseTabun);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseTabun);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseTabun);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseTabun);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseTabun);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseTabun);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseTabun);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseTabun);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseTabun);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseTabun);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseTabun);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseTabun);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseTabun);
    }
    public void PlaySoldierUseSmoke(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderUseSmoke);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanUseSmoke);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorUseSmoke);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerUseSmoke);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderUseSmoke);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistUseSmoke);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerUseSmoke);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerUseSmoke);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutUseSmoke);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanUseSmoke);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorUseSmoke);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeUseSmoke);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterUseSmoke);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneUseSmoke);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerUseSmoke);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfUseSmoke);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesUseSmoke);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatUseSmoke);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianUseSmoke);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicUseSmoke);
    }
    public void PlaySoldierKillEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderKillEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanKillEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorKillEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerKillEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderKillEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistKillEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerKillEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerKillEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutKillEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanKillEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorKillEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeKillEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterKillEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneKillEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerKillEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfKillEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesKillEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatKillEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianKillEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicKillEnemy);
    }
    public void PlaySoldierMeleeBreakeven(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderMeleeBreakeven);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanMeleeBreakeven);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorMeleeBreakeven);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerMeleeBreakeven);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderMeleeBreakeven);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistMeleeBreakeven);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerMeleeBreakeven);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerMeleeBreakeven);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutMeleeBreakeven);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanMeleeBreakeven);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorMeleeBreakeven);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeMeleeBreakeven);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterMeleeBreakeven);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneMeleeBreakeven);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerMeleeBreakeven);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfMeleeBreakeven);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesMeleeBreakeven);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatMeleeBreakeven);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianMeleeBreakeven);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicMeleeBreakeven);
    }
    public void PlaySoldierMeleeMove(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderMeleeMove);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanMeleeMove);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorMeleeMove);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerMeleeMove);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderMeleeMove);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistMeleeMove);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerMeleeMove);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerMeleeMove);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutMeleeMove);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanMeleeMove);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorMeleeMove);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeMeleeMove);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterMeleeMove);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneMeleeMove);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerMeleeMove);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfMeleeMove);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesMeleeMove);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatMeleeMove);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianMeleeMove);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicMeleeMove);
    }
    public void PlaySoldierShotMiss(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderShotMiss);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanShotMiss);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorShotMiss);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerShotMiss);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderShotMiss);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistShotMiss);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerShotMiss);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerShotMiss);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutShotMiss);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanShotMiss);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorShotMiss);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeShotMiss);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterShotMiss);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneShotMiss);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerShotMiss);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfShotMiss);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesShotMiss);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatShotMiss);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianShotMiss);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicShotMiss);
    }
    public void PlaySoldierPickupUHF(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPickupUHF);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPickupUHF);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPickupUHF);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPickupUHF);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPickupUHF);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPickupUHF);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPickupUHF);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPickupUHF);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPickupUHF);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPickupUHF);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPickupUHF);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePickupUHF);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPickupUHF);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePickupUHF);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPickupUHF);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPickupUHF);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPickupUHF);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPickupUHF);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPickupUHF);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPickupUHF);
    }
    public void PlaySoldierPickupULF(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderPickupULF);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanPickupULF);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorPickupULF);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerPickupULF);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderPickupULF);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistPickupULF);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerPickupULF);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerPickupULF);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutPickupULF);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanPickupULF);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorPickupULF);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakePickupULF);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterPickupULF);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cyclonePickupULF);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerPickupULF);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfPickupULF);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesPickupULF);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatPickupULF);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianPickupULF);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicPickupULF);
    }
    public void PlaySoldierSeeEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSeeEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanSeeEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorSeeEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerSeeEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderSeeEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistSeeEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSeeEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerSeeEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutSeeEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanSeeEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSeeEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSeeEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSeeEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSeeEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerSeeEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfSeeEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesSeeEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSeeEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianSeeEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicSeeEnemy);
    }
    public void PlaySoldierSuppressEnemy(string specialty)
    {
        if (specialty.Equals("Leadership"))
            PlayRandomVoice(commanderSuppressEnemy);
        else if (specialty.Equals("Health"))
            PlayRandomVoice(spartanSuppressEnemy);
        else if (specialty.Equals("Resilience"))
            PlayRandomVoice(survivorSuppressEnemy);
        else if (specialty.Equals("Speed"))
            PlayRandomVoice(runnerSuppressEnemy);
        else if (specialty.Equals("Evasion"))
            PlayRandomVoice(evaderSuppressEnemy);
        else if (specialty.Equals("Fight"))
            PlayRandomVoice(reservistSuppressEnemy);
        else if (specialty.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSuppressEnemy);
        else if (specialty.Equals("Camouflage"))
            PlayRandomVoice(seekerSuppressEnemy);
        else if (specialty.Equals("Sight Radius"))
            PlayRandomVoice(scoutSuppressEnemy);
        else if (specialty.Equals("Rifle"))
            PlayRandomVoice(infantrymanSuppressEnemy);
        else if (specialty.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSuppressEnemy);
        else if (specialty.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSuppressEnemy);
        else if (specialty.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSuppressEnemy);
        else if (specialty.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSuppressEnemy);
        else if (specialty.Equals("Shotgun"))
            PlayRandomVoice(hammerSuppressEnemy);
        else if (specialty.Equals("Melee"))
            PlayRandomVoice(wolfSuppressEnemy);
        else if (specialty.Equals("Strength"))
            PlayRandomVoice(herculesSuppressEnemy);
        else if (specialty.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSuppressEnemy);
        else if (specialty.Equals("Electronics"))
            PlayRandomVoice(technicianSuppressEnemy);
        else if (specialty.Equals("Healing"))
            PlayRandomVoice(medicSuppressEnemy);
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







    //soldier selection dialogue
    public void UnsetAllSoldierSelectionSoundFlags(Soldier soldier)
    {
        soldierTakenDamage.Remove(soldier.Id);
        soldierAllyKilledJA.Remove(soldier.Id);
        soldierAllyKilledOrUncon.Remove(soldier.Id);
        soldierEnemyUseULF.Remove(soldier.Id);
        soldierEnemyUseUHF.Remove(soldier.Id);
        soldierEnemyUseTabun.Remove(soldier.Id);
        soldierEnemyUseSmoke.Remove(soldier.Id);
        soldierHeardSound.Remove(soldier.Id);
        soldierEnemyKilledJA.Remove(soldier.Id);
        soldierEnemyKilled.Remove(soldier.Id);
        soldierAllyMissesShot.Remove(soldier.Id);
        soldierEnemyMissesShot.Remove(soldier.Id);
        soldierAllyUseULF.Remove(soldier.Id);
        soldierAllyUseUHF.Remove(soldier.Id);
    }
    public void SetSoundFlag(Soldier soldier, List<string> soldierList)
    {
        if (soldier.IsConscious())
        {
            if (!soldierList.Contains(soldier.Id))
                soldierList.Add(soldier.Id);
        }
    }
    public void SetSoldierSelectionSoundFlagAfterDamage(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierTakenDamage);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledJA(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyKilledJA);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyKilledOrUncon(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyKilledOrUncon);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseULF(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyUseULF);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseUHF(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyUseUHF);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseTabun(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyUseTabun);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyUseSmoke(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyUseSmoke);
    }
    public void SetSoldierSelectionSoundFlagAfterHeardSound(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierHeardSound);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyKilledJA(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyKilledJA);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyKilled(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyKilled);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyMissesShot(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyMissesShot);
    }
    public void SetSoldierSelectionSoundFlagAfterEnemyMissesShot(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierEnemyMissesShot);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyUseULF(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyUseULF);
    }
    public void SetSoldierSelectionSoundFlagAfterAllyUseUHF(Soldier soldier)
    {
        SetSoundFlag(soldier, soldierAllyUseUHF);
    }
    public void PlaySoldierSelection(Soldier soldier)
    {
        if (soldier.IsConscious())
        {
            if (soldierTakenDamage.Contains(soldier.Id))
                PlaySoldierSelectionAfterDamage(soldier);
            else if (soldierAllyKilledJA.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyKilledJA(soldier);
            else if (soldierAllyKilledOrUncon.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyKilledOrUncon(soldier);
            else if (soldierEnemyUseULF.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyUseULF(soldier);
            else if (soldierEnemyUseUHF.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyUseUHF(soldier);
            else if (soldierEnemyUseTabun.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyUseTabun(soldier);
            else if (soldierEnemyUseSmoke.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyUseSmoke(soldier);
            else if (soldierHeardSound.Contains(soldier.Id))
                PlaySoldierSelectionAfterHeardSound(soldier);
            else if (soldierEnemyKilledJA.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyKilledJA(soldier);
            else if (soldierEnemyKilled.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyKilled(soldier);
            else if (soldierAllyMissesShot.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyMissesShot(soldier);
            else if (soldierEnemyMissesShot.Contains(soldier.Id))
                PlaySoldierSelectionAfterEnemyMissesShot(soldier);
            else if (soldierAllyUseULF.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyUseULF(soldier);
            else if (soldierAllyUseUHF.Contains(soldier.Id))
                PlaySoldierSelectionAfterAllyUseUHF(soldier);
            else if (soldier.HasGunsEquipped() && soldier.HasOneAmmo())
                PlaySoldierSelectionWhileOneAmmo(soldier);
            else if (soldier.HasGunsEquipped() && soldier.HasNoAmmo())
                PlaySoldierSelectionWhileNoAmmo(soldier);
            else if (soldier.HasBrokenAllies())
                PlaySoldierSelectionWhileAllyBroken(soldier);
            else
                PlaySoldierSelectionGeneric(soldier);

            
            
        }

        //unset other flags
        UnsetAllSoldierSelectionSoundFlags(soldier);
    }
    public void PlaySoldierSelectionAfterDamage(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterDamage);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterDamage);
    }
    public void PlaySoldierSelectionAfterAllyKilledJA(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyKilledJA);
    }
    public void PlaySoldierSelectionAfterAllyKilledOrUncon(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyKilledOrUncon);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyKilledOrUncon);
    }
    public void PlaySoldierSelectionAfterEnemyUseULF(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyUseULF);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyUseULF);
    }
    public void PlaySoldierSelectionAfterEnemyUseUHF(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyUseUHF);
    }
    public void PlaySoldierSelectionAfterEnemyUseTabun(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyUseTabun);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyUseTabun);
    }
    public void PlaySoldierSelectionAfterEnemyUseSmoke(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyUseSmoke);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyUseSmoke);
    }
    public void PlaySoldierSelectionAfterHeardSound(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterHeardSound);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterHeardSound);
    }
    public void PlaySoldierSelectionAfterEnemyKilledJA(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyKilledJA);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyKilledJA);
    }
    public void PlaySoldierSelectionAfterEnemyKilled(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyKilled);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyKilled);
    }
    public void PlaySoldierSelectionAfterAllyMissesShot(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyMissesShot);
    }
    public void PlaySoldierSelectionAfterEnemyMissesShot(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterEnemyMissesShot);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterEnemyMissesShot);
    }
    public void PlaySoldierSelectionAfterAllyUseULF(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyUseULF);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyUseULF);
    }
    public void PlaySoldierSelectionAfterAllyUseUHF(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionAfterAllyUseUHF);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionAfterAllyUseUHF);
    }
    public void PlaySoldierSelectionWhileOneAmmo(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionWhileOneAmmo);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionWhileOneAmmo);
    }
    public void PlaySoldierSelectionWhileNoAmmo(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionWhileNoAmmo);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionWhileNoAmmo);
    }
    public void PlaySoldierSelectionWhileAllyBroken(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionWhileAllyBroken);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionWhileAllyBroken);
    }
    public void PlaySoldierSelectionGeneric(Soldier soldier)
    {
        if (soldier.soldierSpeciality.Equals("Leadership"))
            PlayRandomVoice(commanderSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Health"))
            PlayRandomVoice(spartanSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Resilience"))
            PlayRandomVoice(survivorSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Speed"))
            PlayRandomVoice(runnerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Evasion"))
            PlayRandomVoice(evaderSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Fight"))
            PlayRandomVoice(reservistSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Perceptiveness"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Camouflage"))
            PlayRandomVoice(seekerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sight Radius"))
            PlayRandomVoice(scoutSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Rifle"))
            PlayRandomVoice(infantrymanSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Assault Rifle"))
            PlayRandomVoice(operatorSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Light Machine Gun"))
            PlayRandomVoice(earthquakeSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sniper Rifle"))
            PlayRandomVoice(hunterSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Sub-Machine Gun"))
            PlayRandomVoice(cycloneSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Shotgun"))
            PlayRandomVoice(hammerSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Melee"))
            PlayRandomVoice(wolfSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Strength"))
            PlayRandomVoice(herculesSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Diplomacy"))
            PlayRandomVoice(diplomatSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Electronics"))
            PlayRandomVoice(technicianSelectionGeneric);
        else if (soldier.soldierSpeciality.Equals("Healing"))
            PlayRandomVoice(medicSelectionGeneric);
    }
}
