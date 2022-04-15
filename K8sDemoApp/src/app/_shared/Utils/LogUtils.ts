import { LogTypeEnum } from "src/app/_enum/LogTypeEnum";

export class LogUtils {

    public ExtractDateString(logstring: string):string
    {
        var splitted = logstring.split("|"); 
        return splitted[0];
    }

    public ExtractMessage(logstring: string):string
    {
        var splitted = logstring.split("|"); 
        return logstring.replace(splitted[0], "");
    }

    
    public ConvertLogType(backendLogType: LogTypeEnum)
    {
        switch(backendLogType) { 
            case LogTypeEnum.Error: { 
                return 'ERR' 
            } 
            case LogTypeEnum.Warning: { 
                return 'WARN' 
            } 
            default: { 
                return 'INFO' 
            } 
        } 
    }
}