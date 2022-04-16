import { RoleEnum } from "../_enum/RoleEnum";

export interface LoggedUser{
    username: string;
    token:string;
    roles: RoleEnum[];
}

export interface LoginRequest {
    username: string;
    password: string;
}