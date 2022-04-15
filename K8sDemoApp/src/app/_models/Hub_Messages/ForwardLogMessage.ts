import { LogTypeEnum } from "../../_enum/LogTypeEnum";

export interface ForwardLogMessage{
    program: string;
    messageType:LogTypeEnum;
    message:string;
}