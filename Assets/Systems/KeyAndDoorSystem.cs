using UnityEngine;
using FYFY;

public class KeyAndDoorSystem : FSystem {

	private GameData gameData = FamilyManager.getFamily(new AllOfComponents(typeof(GameData))).First().GetComponent<GameData>();
	private Family f_players = FamilyManager.getFamily(new AnyOfTags("Player"));
	private Family f_door = FamilyManager.getFamily(new AnyOfTags("LockedDoor"));
	private Family f_backpack = FamilyManager.getFamily(new AllOfComponents(typeof(Backpack)));

	public static KeyAndDoorSystem instance;

	public KeyAndDoorSystem()
	{
		instance = this;
	}
	
	// Use to init system before the first onProcess call
	protected override void onStart(){
	}

	// Use to process your families.
	protected override void onProcess(int familiesUpdateCount) {

		foreach(GameObject door in f_door){

			var slot_id = door.GetComponent<ActivationSlot>().slotID;
			var door_pos = door.GetComponent<Position>();
			var door_orientation = (int)door.GetComponent<Direction>().direction;

			foreach(GameObject player in f_players){	
				var pos = player.GetComponent<Position>();
				var direction = (int)player.GetComponent<Direction>().direction;

				bool faced = playerFacesDoor(direction, pos.x, pos.y, door_orientation, door_pos.x, door_pos.y);

				if(faced){
					var backp = f_backpack.First().GetComponent<Backpack>().available_slots;
					foreach(var item in backp){
						if(item.Item1 == "key" && item.Item2 == slot_id){
							closeDoor(door);
						}
					}
				}
			}
		}
	}

	private bool playerFacesDoor(int robot_ori, int robot_posX, int robot_posY, int door_ori, int door_posX, int door_posY){
		if(door_ori == 0 || door_ori == 1){
			if(robot_ori == 2 || robot_ori == 3 || robot_posX != door_posX){
				return false;
			}
			else if((robot_posY == door_posY - 1 && robot_ori == 1)||(robot_posY == door_posY + 1 && robot_ori == 0)){
				return true;
			}
		}
		else{
			if(robot_ori == 0 || robot_ori == 1 || robot_posY != door_posY){
				return false;
			}
			else if((robot_posX == door_posX - 1 && robot_ori == 2)||(robot_posX == door_posX + 1 && robot_ori  == 3)){
				return true;
			}
		}
	
		return false;

	}

	private void closeDoor(GameObject door){
		door.transform.parent.GetComponent<AudioSource>().Play();
		door.transform.parent.GetComponent<Animator>().SetTrigger("Open");
		door.transform.parent.GetComponent<Animator>().speed = gameData.gameSpeed_current;
	}
}