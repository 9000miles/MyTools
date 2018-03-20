
namespace BaiduARInternal{
	
	public class BaiduARCamera : ARCamera {
		
		void Awake(){
			BaiduARBuild.Instance.Start ();
		}
}
}
