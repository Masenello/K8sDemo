import { Input } from "@angular/core";
import { LogTypeEnum } from "../_enum/LogTypeEnum";

export class LogTypeConverter {

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