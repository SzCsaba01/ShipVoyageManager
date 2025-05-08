import { IVoyage } from "./voyage.model";

export interface IPaginatedVoyages {
    voyages: IVoyage[];
    totalCount: number;
}