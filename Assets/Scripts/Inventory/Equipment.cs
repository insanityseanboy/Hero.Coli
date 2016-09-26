using UnityEngine;
using System.Collections.Generic;

public class Equipment : DeviceContainer
{
    //////////////////////////////// singleton fields & methods ////////////////////////////////
    private const string gameObjectName = "DeviceEquipment";
    private static Equipment _instance;
    public static Equipment get()
    {
        if (_instance == null)
        {
            // Debug.LogWarning("Equipment::get was badly initialized");
            _instance = GameObject.Find(gameObjectName).GetComponent<Equipment>();
        }
        return _instance;
    }
    
    void Awake()
    {
        Debug.Log(this.GetType() + " Awake");
        if((_instance != null) && (_instance != this))
        {            
            Debug.LogError(this.GetType() + " has two running instances");
        }
        else
        {
            _instance = this;
            initializeIfNecessary();
        }
    }

    void OnDestroy()
    {
        Debug.Log(this.GetType() + " OnDestroy " + (_instance == this));
       _instance = (_instance == this) ? null : _instance;
    }

    private bool _initialized = false;  
    private void initializeIfNecessary()
    {
        if(!_initialized)
        {
            _initialized = true;
        }
    }

    new void Start()
    {
        base.Start();
        // Debug.Log("Equipment::Start()");
        _reactionEngine = ReactionEngine.get();
    }
    ////////////////////////////////////////////////////////////////////////////////////////////

    private ReactionEngine _reactionEngine;
    public int _celliaMediumID = 1;

    public Equipment()
    {
        //by default, nothing's equiped
        _devices = new List<Device>();
    }

    private void addToReactionEngine(Device device)
    {
        // Debug.Log("Equipment::addToReactionEngine reactions from device "+device.getInternalName()+" ("+device.ToString ()+")");

        LinkedList<IReaction> reactions = device.getReactions();
        // Debug.Log("Equipment::addToReactionEngine reactions="+Logger.ToString<IReaction>(reactions)+" from "+device);

        foreach (IReaction reaction in reactions)
        {
            // Debug.Log("Equipment::addToReactionEngine adding reaction="+reaction);
            _reactionEngine.addReactionToMedium(_celliaMediumID, reaction);
        }
    }

    public override AddingResult askAddDevice(Device device, bool reportToRedMetrics = false)
    {
        if (device == null)
        {
            // Debug.LogWarning("Equipment::askAddDevice device == null");
            return AddingResult.FAILURE_DEFAULT;
        }
        Device copy = Device.buildDevice(device);
        if (copy == null)
        {
            // Debug.LogWarning("Equipment::askAddDevice copy == null");
            return AddingResult.FAILURE_DEFAULT;
        }

        //TODO test BioBricks equality (cf next line)
        if (_devices.Exists(d => d.Equals(copy)))
        //if(_devices.Exists(d => d.getInternalName() == copy.getInternalName()))
        {
            // Debug.Log("Equipment::askAddDevice device already present");
            return AddingResult.FAILURE_SAME_DEVICE;
        }
        _devices.Add(copy);
        safeGetDisplayer().addEquipedDevice(copy);
        // TODO replace by event broadcasting
        if (!PhenoAmpicillinProducer.get().onEquippedDevice(device))
        {
            addToReactionEngine(copy);
        }
        return AddingResult.SUCCESS;
    }

    //TODO
    private void removeFromReactionEngine(Device device)
    {
        // Debug.Log("Equipment::removeFromReactionEngine reactions from device "+device);

        LinkedList<IReaction> reactions = device.getReactions();
        // Debug.Log("Equipment::removeFromReactionEngine device implies reactions="+Logger.ToString<IReaction>(reactions));

        //LinkedList<Medium> mediums = _reactionEngine.getMediumList();
        //Medium celliaMedium = ReactionEngine.getMediumFromId(_celliaMediumID, mediums);

        //LinkedList<IReaction> celliaReactions = celliaMedium.getReactions();
        // Debug.Log("Equipment::removeFromReactionEngine initialCelliaReactions="+Logger.ToString<IReaction>(celliaReactions)
        //  );

        foreach (IReaction reaction in reactions)
        {
            // Debug.Log("Equipment::removeFromReactionEngine removing reaction="+reaction);
            _reactionEngine.removeReaction(_celliaMediumID, reaction, false);
        }

        //celliaReactions = celliaMedium.getReactions();
        // Debug.Log("Equipment::removeFromReactionEngine finalCelliaReactions="+Logger.ToString<IReaction>(celliaReactions)
        //  );
    }

    public override void removeDevice(Device device)
    {
        // Debug.Log("Equipment removeDevice("+device+") start");
        if (_devices.Contains(device))
        {
            _devices.RemoveAll(d => d.Equals(device));
            safeGetDisplayer().removeEquippedDevice(device);
            removeFromReactionEngine(device);
            LimitedBiobricksCraftZoneManager.get().unequip(device);
            PhenoAmpicillinProducer.get().onUnequippedDevice(device);
        }
    }

    public void removeDevice(Device device, bool removeBricks)
    {
        removeDevice(device);
    }

    // not optimized
    public override void removeDevices(List<Device> toRemove)
    {
        // Debug.Log("Equipment::removeDevices");

        foreach (Device device in toRemove)
        {
            removeDevice(device);
        }
    }

    public override void editDevice(Device device)
    {
        //TODO
        Debug.LogError("Equipment::editeDevice NOT IMPLEMENTED");
    }

    public override string ToString()
    {
        string res = "";
        foreach (Device d in _devices)
        {
            if (res != "")
            {
                res = res + "; ";
            }
            res = res + d.ToString();
        }
        res = "[Equipment " + res + "]";
        return res;
    }
}

