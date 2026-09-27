"use client";

import { useState } from "react";
import { Period } from "../lib/definitions";
import HelloUser from "./HelloUser";
import PeriodButtons from "./PeriodButtons";
import TotalBalanceCard from "./TotalBalanceCard";
import TransactionCard from "./TransactionCard";
import ActionCard from "./ActionCard";

export default function Dashboard() {
  const [period, setPeriod] = useState<Period | null>(null);

  const handlePeriodSelect = (period: Period) => {
    setPeriod(period);
  };

  return (
    <>
      <div className="flex justify-between">
        <HelloUser></HelloUser>
        <PeriodButtons onPeriodSelect={handlePeriodSelect}></PeriodButtons>
      </div>
      <div className="grid grid-cols-1 sm:grid-cols-3 gap-8 mt-8">
        <TotalBalanceCard></TotalBalanceCard>
        <TransactionCard
          title="Incomes"
          endpoint="/transaction/incomes"
          period={period || { startDate: "", endDate: "" }}
        />
        <TransactionCard
          title="Expenses"
          endpoint="/transaction/expenses"
          period={period || { startDate: "", endDate: "" }}
        />

        <ActionCard
          imgUrl="add_income_icon.png"
          title="Add income"
          text="Create an income manually"
        />
        <ActionCard
          imgUrl="add_expense_icon.png"
          title="Add expense"
          text="Create an expense manually"
        />
        <div className="opacity-50 pointer-events-none cursor-not-allowed select-none">
          <ActionCard
            imgUrl="add_transfer_icon.png"
            title="Transfer money"
            text="Select the amount and make a transfer"
          />
        </div>
      </div>
    </>
  );
}
