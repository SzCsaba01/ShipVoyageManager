import { IShip } from "./ship.model";

export interface IFilteredPaginatedShips {
    ships: IShip[];
    totalCount: number;
}