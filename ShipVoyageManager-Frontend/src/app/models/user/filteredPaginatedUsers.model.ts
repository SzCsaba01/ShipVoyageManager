import { IUser } from "./user.model";

export interface IFilteredPaginatedUsers {
    users: IUser[];
    totalCount: number;
}