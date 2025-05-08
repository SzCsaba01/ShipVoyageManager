import { Guid } from "guid-typescript";

export interface IUser {
    id: Guid;
    username: string;
    email: string;
    isEmailConfirmed: boolean;
    registrationDate: Date; 
}