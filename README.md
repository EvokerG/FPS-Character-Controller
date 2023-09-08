# FPS Character Controller
 Universal script for Unity for handling player movement and gun positioning
![image](https://github.com/EvokerG/FPS-Character-Controller/assets/142021665/f7594bd4-e766-4cab-af81-16517ce89487)

Setup process:
    GameObject inserted in "Stamina bar" will scale from 0 to 200% by x coordinate depending on stamina level. This was initially made for two objects:    
    
   ![image](https://github.com/EvokerG/FPS-Character-Controller/assets/142021665/f0930609-ff07-4976-b02a-e4e92074f25d)
   
  Stamina Bar is background object and Slider has "Maskable" paramether enabled in Image component

  To guarantee everything is working follow this structure

  ![image](https://github.com/EvokerG/FPS-Character-Controller/assets/142021665/bf42dbed-0d9a-4ef1-b297-f05c4de6ec9a)

   PlayerObject ▽
         PlayerViewCamera ▽
                      PlayerGun ▽
                           GunAttachments
       
