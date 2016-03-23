package
{
	import flash.desktop.NativeProcess;
	import flash.desktop.NativeProcessStartupInfo;
	import flash.display.Sprite;
	import flash.events.ProgressEvent;
	import flash.filesystem.File;
	import flash.text.TextField;
	import flash.text.TextFormat;
	
	/**
	 * ...
	 * @author 
	 */
	public class Main extends Sprite 
	{
		
		private var _fileName:String = 'Keylogger/WM_INPUT.exe';// C#アプリケーション
		private var _file:File = File.applicationDirectory.resolvePath(_fileName);
		private var _nativeProcess:NativeProcess;
		private var _textField:TextField = new TextField();
		public function Main() 
		{
			_textField.defaultTextFormat = new TextFormat("_sans", 24);
			_textField.width = stage.stageWidth;
			_textField.height = stage.stageHeight;
			_textField.text = "start";
			_textField.multiline = _textField.wordWrap = true;
			addChild(_textField);
			
			
			var text:String = "";
			var arguments:Vector.<String>;
			var nativeProcessStartupInfo:NativeProcessStartupInfo;
			if (NativeProcess.isSupported && _file.exists) {
				
				// 引数（不要なら付けなくても良い）
				//arguments = new Vector.<String>();
				//arguments[0] = 'from Adobe AIR';
				//arguments[1] = 'Boo';
				
				nativeProcessStartupInfo = new NativeProcessStartupInfo();
				//nativeProcessStartupInfo.arguments = arguments;
				nativeProcessStartupInfo.executable = _file;
				
				_nativeProcess = new NativeProcess();
				_nativeProcess.addEventListener(ProgressEvent.STANDARD_OUTPUT_DATA, nativeProcess_standardOutputData);
				_nativeProcess.addEventListener(ProgressEvent.STANDARD_INPUT_PROGRESS, nativeProcess_standardInputProgress);
				_nativeProcess.addEventListener(ProgressEvent.STANDARD_ERROR_DATA, nativeProcess_standardErrorData);
				_nativeProcess.start(nativeProcessStartupInfo);
				_textField.text = "NativeProcess start";
			}else {
				trace(NativeProcess.isSupported, "ネイティブプロセスをサポートしていません。");
				_textField.text = "ネイティブプロセスをサポートしていません。";
			}
			
		}
		
		private function nativeProcess_standardOutputData(e:ProgressEvent):void 
		{
			var str:String = _nativeProcess.standardOutput.readUTFBytes(_nativeProcess.standardOutput.bytesAvailable);
			_textField.text = str;
			
		}
		
		private function nativeProcess_standardErrorData(e:ProgressEvent):void 
		{
			_textField.text = "nativeProcess_standardErrorData";
		}
		
		private function nativeProcess_standardInputProgress(e:ProgressEvent):void 
		{
			_textField.text = "nativeProcess_standardInputProgress";
		}	
		
	}
	
}