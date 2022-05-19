import { RoleDto } from "./RoleDto";

export class UserDto{
    username: string;
    firstName:string;
    lastName:string;
    department:string;
    mail:string;
    token:string;
    roles:RoleDto[];
}