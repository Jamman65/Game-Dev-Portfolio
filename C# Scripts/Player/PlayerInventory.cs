﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
namespace JO
{

    public class PlayerInventory : CharacterInventoryManager
    {
        //public SpellItem currentSpell;
        //public ConsumableItem currentConsumable;

        //[Header("Current Equipment")]
        //public HelmetEquipment currentHelmetEquipment;
        //public TorsoEquipment currentTorsoEquipment;
        //public LegEquipment currentLegEquipment;
        //public HandEquipment currentHandEquipment;

        WeaponSlotManager weaponSlotManager;
        //public WeaponItem rightWeapon;
        //public WeaponItem leftWeapon;
        //public WeaponItem unarmedWeapon;
     


        //public WeaponItem[] weaponsInRightHandSlots = new WeaponItem[1];
        //public WeaponItem[] weaponsInLeftHandSlots = new WeaponItem[1];

        //public int currentRightWeaponIndex = 0;
        //public int currentLeftWeaponIndex = 0;



        public List<WeaponItem> weaponsInventory;
        public List<HelmetEquipment> helmetEquipmentInventory;
        public List<TorsoEquipment> torsoEquipmentInventory;
        public List<LegEquipment> legEquipmentInventory;
        public List<HandEquipment> HandEquipmentInventory;


        private void Awake()
        {
            weaponSlotManager = GetComponentInChildren<WeaponSlotManager>();
           
        }

        private void Start()
        {
            //rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
            //leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];

            //weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            //weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);

            //rightWeapon = unarmedWeapon;
            //leftWeapon = unarmedWeapon;

            rightWeapon = weaponsInRightHandSlots[0];
            leftWeapon = weaponsInLeftHandSlots[0];
            weaponSlotManager.LoadWeaponOnSlot(rightWeapon, false);
            weaponSlotManager.LoadWeaponOnSlot(leftWeapon, true);


        }

        public void ChangeRightWeapon()
        {
            currentRightWeaponIndex = currentRightWeaponIndex + 1;

            if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }

            else if(currentRightWeaponIndex == 0 && weaponsInRightHandSlots[0] == null)
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;
            }

            else if(currentRightWeaponIndex == 1 && weaponsInRightHandSlots[1] != null)
            {
                rightWeapon = weaponsInRightHandSlots[currentRightWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInRightHandSlots[currentRightWeaponIndex], false);
            }

            else
            {
                currentRightWeaponIndex = currentRightWeaponIndex + 1;

            }

            if(currentRightWeaponIndex > weaponsInRightHandSlots.Length - 1)
            {
                currentRightWeaponIndex = -1;
                rightWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, false);
            }
        }

        public void ChangeLeftWeapon()
        {

            currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }

            else if (currentLeftWeaponIndex == 0 && weaponsInLeftHandSlots[0] == null)
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;
            }

            else if (currentLeftWeaponIndex == 1 && weaponsInLeftHandSlots[1] != null)
            {
                leftWeapon = weaponsInLeftHandSlots[currentLeftWeaponIndex];
                weaponSlotManager.LoadWeaponOnSlot(weaponsInLeftHandSlots[currentLeftWeaponIndex], true);
            }

            else
            {
                currentLeftWeaponIndex = currentLeftWeaponIndex + 1;

            }

            if (currentLeftWeaponIndex > weaponsInLeftHandSlots.Length - 1)
            {
                currentLeftWeaponIndex = -1;
                leftWeapon = unarmedWeapon;
                weaponSlotManager.LoadWeaponOnSlot(unarmedWeapon, true);
            }
        }
    }
}
