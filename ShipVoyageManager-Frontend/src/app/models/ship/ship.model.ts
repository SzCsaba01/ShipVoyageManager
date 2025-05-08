import { Guid } from "guid-typescript";
import { IVoyage } from "../voyage/voyage.model";

export interface IShip {
    id: Guid;
    name: string;
    maxSpeed: number;
    voyages: IVoyage[];
}