namespace MarketManagement.UserControls
{
    partial class Billing
    {
        /// <summary> 
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary> 
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.btn_remove_selected = new System.Windows.Forms.Button();
            this.btn_save = new System.Windows.Forms.Button();
            this.btn_addtocard = new System.Windows.Forms.Button();
            this.txt_productprice = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txt_contact = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txt_productId = new System.Windows.Forms.TextBox();
            this.txt_customerid = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.lbl_productid = new System.Windows.Forms.Label();
            this.txt_productquantity = new System.Windows.Forms.TextBox();
            this.txt_address = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.panel2 = new System.Windows.Forms.Panel();
            this.txt_invoiceno = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.dtp_invoicedate = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.panel3 = new System.Windows.Forms.Panel();
            this.lbl_customerid = new System.Windows.Forms.Label();
            this.panel4 = new System.Windows.Forms.Panel();
            this.btn_removecart = new System.Windows.Forms.Button();
            this.textBox3 = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.btn_update = new System.Windows.Forms.Button();
            this.btn_print = new System.Windows.Forms.Button();
            this.txt_totalprice = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.panel5 = new System.Windows.Forms.Panel();
            this.db_dataGridView1 = new System.Windows.Forms.DataGridView();
            this.txt_productname = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txt_customername = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.productNameDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.quantityDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.priceDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.descriptionDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.categoryDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.idDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.baseProductBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.panel2.SuspendLayout();
            this.panel3.SuspendLayout();
            this.panel4.SuspendLayout();
            this.panel5.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.db_dataGridView1)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseProductBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // btn_remove_selected
            // 
            this.btn_remove_selected.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(221)))), ((int)(((byte)(143)))));
            this.btn_remove_selected.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_remove_selected.FlatAppearance.BorderSize = 2;
            this.btn_remove_selected.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(111)))), ((int)(((byte)(0)))));
            this.btn_remove_selected.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(219)))));
            this.btn_remove_selected.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_remove_selected.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_remove_selected.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(72)))), ((int)(((byte)(8)))));
            this.btn_remove_selected.Location = new System.Drawing.Point(201, 374);
            this.btn_remove_selected.Name = "btn_remove_selected";
            this.btn_remove_selected.Size = new System.Drawing.Size(132, 41);
            this.btn_remove_selected.TabIndex = 31;
            this.btn_remove_selected.Text = "Remove Item";
            this.btn_remove_selected.UseVisualStyleBackColor = false;
            this.btn_remove_selected.Click += new System.EventHandler(this.btn_remove_selected_Click);
            // 
            // btn_save
            // 
            this.btn_save.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(249)))));
            this.btn_save.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_save.FlatAppearance.BorderSize = 2;
            this.btn_save.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(110)))), ((int)(((byte)(173)))));
            this.btn_save.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(173)))), ((int)(((byte)(217)))));
            this.btn_save.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_save.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_save.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(58)))), ((int)(((byte)(94)))));
            this.btn_save.Location = new System.Drawing.Point(63, 374);
            this.btn_save.Name = "btn_save";
            this.btn_save.Size = new System.Drawing.Size(113, 41);
            this.btn_save.TabIndex = 30;
            this.btn_save.Text = "Save";
            this.btn_save.UseVisualStyleBackColor = false;
            this.btn_save.Click += new System.EventHandler(this.btn_save_Click);
            // 
            // btn_addtocard
            // 
            this.btn_addtocard.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.btn_addtocard.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_addtocard.FlatAppearance.BorderSize = 2;
            this.btn_addtocard.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(50)))), ((int)(((byte)(93)))));
            this.btn_addtocard.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_addtocard.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_addtocard.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_addtocard.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_addtocard.Location = new System.Drawing.Point(946, 24);
            this.btn_addtocard.Name = "btn_addtocard";
            this.btn_addtocard.Size = new System.Drawing.Size(132, 41);
            this.btn_addtocard.TabIndex = 29;
            this.btn_addtocard.Text = "Add to Cart";
            this.btn_addtocard.UseVisualStyleBackColor = false;
            this.btn_addtocard.Click += new System.EventHandler(this.btn_addtocard_Click);
            // 
            // txt_productprice
            // 
            this.txt_productprice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_productprice.Location = new System.Drawing.Point(328, 37);
            this.txt_productprice.Name = "txt_productprice";
            this.txt_productprice.Size = new System.Drawing.Size(262, 30);
            this.txt_productprice.TabIndex = 28;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Cursor = System.Windows.Forms.Cursors.Default;
            this.label4.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label4.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label4.Location = new System.Drawing.Point(325, 12);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(132, 23);
            this.label4.TabIndex = 27;
            this.label4.Text = "Product Price";
            // 
            // txt_contact
            // 
            this.txt_contact.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_contact.Location = new System.Drawing.Point(328, 36);
            this.txt_contact.Name = "txt_contact";
            this.txt_contact.Size = new System.Drawing.Size(262, 30);
            this.txt_contact.TabIndex = 26;
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label5.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(211)))), ((int)(((byte)(57)))));
            this.label5.Location = new System.Drawing.Point(325, 11);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(146, 23);
            this.label5.TabIndex = 25;
            this.label5.Text = "Phone Number";
            // 
            // txt_productId
            // 
            this.txt_productId.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_productId.Location = new System.Drawing.Point(16, 37);
            this.txt_productId.Name = "txt_productId";
            this.txt_productId.Size = new System.Drawing.Size(262, 30);
            this.txt_productId.TabIndex = 22;
            // 
            // txt_customerid
            // 
            this.txt_customerid.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_customerid.Location = new System.Drawing.Point(16, 36);
            this.txt_customerid.Name = "txt_customerid";
            this.txt_customerid.Size = new System.Drawing.Size(262, 30);
            this.txt_customerid.TabIndex = 20;
            this.txt_customerid.TextChanged += new System.EventHandler(this.txt_customerid_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label2.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.label2.Location = new System.Drawing.Point(-371, -13);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(121, 23);
            this.label2.TabIndex = 19;
            this.label2.Text = "Customer ID";
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(61, 4);
            // 
            // lbl_productid
            // 
            this.lbl_productid.AutoSize = true;
            this.lbl_productid.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_productid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(211)))), ((int)(((byte)(57)))));
            this.lbl_productid.Location = new System.Drawing.Point(13, 12);
            this.lbl_productid.Name = "lbl_productid";
            this.lbl_productid.Size = new System.Drawing.Size(104, 23);
            this.lbl_productid.TabIndex = 21;
            this.lbl_productid.Text = "Product ID";
            // 
            // txt_productquantity
            // 
            this.txt_productquantity.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_productquantity.Location = new System.Drawing.Point(640, 37);
            this.txt_productquantity.Name = "txt_productquantity";
            this.txt_productquantity.Size = new System.Drawing.Size(262, 30);
            this.txt_productquantity.TabIndex = 38;
            this.txt_productquantity.TextChanged += new System.EventHandler(this.txt_productquantity_TextChanged);
            // 
            // txt_address
            // 
            this.txt_address.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_address.Location = new System.Drawing.Point(640, 36);
            this.txt_address.Name = "txt_address";
            this.txt_address.Size = new System.Drawing.Size(262, 30);
            this.txt_address.TabIndex = 36;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label7.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.label7.Location = new System.Drawing.Point(637, 11);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(84, 23);
            this.label7.TabIndex = 35;
            this.label7.Text = "Address";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label8.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(219)))), ((int)(((byte)(211)))), ((int)(((byte)(57)))));
            this.label8.Location = new System.Drawing.Point(637, 12);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(89, 23);
            this.label8.TabIndex = 37;
            this.label8.Text = "Quantity";
            // 
            // panel2
            // 
            this.panel2.Controls.Add(this.txt_invoiceno);
            this.panel2.Controls.Add(this.label6);
            this.panel2.Controls.Add(this.dtp_invoicedate);
            this.panel2.Controls.Add(this.label1);
            this.panel2.Location = new System.Drawing.Point(3, 15);
            this.panel2.Name = "panel2";
            this.panel2.Size = new System.Drawing.Size(925, 81);
            this.panel2.TabIndex = 39;
            // 
            // txt_invoiceno
            // 
            this.txt_invoiceno.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_invoiceno.Location = new System.Drawing.Point(327, 34);
            this.txt_invoiceno.Name = "txt_invoiceno";
            this.txt_invoiceno.Size = new System.Drawing.Size(262, 30);
            this.txt_invoiceno.TabIndex = 24;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label6.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.label6.Location = new System.Drawing.Point(324, 8);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(101, 23);
            this.label6.TabIndex = 23;
            this.label6.Text = "Invoice ID";
            // 
            // dtp_invoicedate
            // 
            this.dtp_invoicedate.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.dtp_invoicedate.Location = new System.Drawing.Point(15, 34);
            this.dtp_invoicedate.Name = "dtp_invoicedate";
            this.dtp_invoicedate.Size = new System.Drawing.Size(262, 30);
            this.dtp_invoicedate.TabIndex = 18;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Cursor = System.Windows.Forms.Cursors.Default;
            this.label1.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label1.Location = new System.Drawing.Point(12, 8);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(127, 23);
            this.label1.TabIndex = 17;
            this.label1.Text = "Invoice Date";
            // 
            // panel3
            // 
            this.panel3.Controls.Add(this.lbl_customerid);
            this.panel3.Controls.Add(this.txt_address);
            this.panel3.Controls.Add(this.label7);
            this.panel3.Controls.Add(this.txt_contact);
            this.panel3.Controls.Add(this.label5);
            this.panel3.Controls.Add(this.txt_customerid);
            this.panel3.Controls.Add(this.label2);
            this.panel3.Location = new System.Drawing.Point(3, 109);
            this.panel3.Name = "panel3";
            this.panel3.Size = new System.Drawing.Size(925, 83);
            this.panel3.TabIndex = 40;
            // 
            // lbl_customerid
            // 
            this.lbl_customerid.AutoSize = true;
            this.lbl_customerid.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbl_customerid.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.lbl_customerid.Location = new System.Drawing.Point(18, 6);
            this.lbl_customerid.Name = "lbl_customerid";
            this.lbl_customerid.Size = new System.Drawing.Size(121, 23);
            this.lbl_customerid.TabIndex = 37;
            this.lbl_customerid.Text = "Customer ID";
            // 
            // panel4
            // 
            this.panel4.Controls.Add(this.txt_productquantity);
            this.panel4.Controls.Add(this.label8);
            this.panel4.Controls.Add(this.txt_productprice);
            this.panel4.Controls.Add(this.label4);
            this.panel4.Controls.Add(this.txt_productId);
            this.panel4.Controls.Add(this.lbl_productid);
            this.panel4.Location = new System.Drawing.Point(3, 203);
            this.panel4.Name = "panel4";
            this.panel4.Size = new System.Drawing.Size(925, 83);
            this.panel4.TabIndex = 41;
            // 
            // btn_removecart
            // 
            this.btn_removecart.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.btn_removecart.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_removecart.FlatAppearance.BorderSize = 2;
            this.btn_removecart.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(201)))), ((int)(((byte)(50)))), ((int)(((byte)(93)))));
            this.btn_removecart.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(192)))));
            this.btn_removecart.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_removecart.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_removecart.ForeColor = System.Drawing.SystemColors.ButtonHighlight;
            this.btn_removecart.Location = new System.Drawing.Point(946, 97);
            this.btn_removecart.Name = "btn_removecart";
            this.btn_removecart.Size = new System.Drawing.Size(132, 41);
            this.btn_removecart.TabIndex = 42;
            this.btn_removecart.Text = "Remove All";
            this.btn_removecart.UseVisualStyleBackColor = false;
            this.btn_removecart.Click += new System.EventHandler(this.btn_removecart_Click);
            // 
            // textBox3
            // 
            this.textBox3.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.textBox3.Location = new System.Drawing.Point(643, 49);
            this.textBox3.Name = "textBox3";
            this.textBox3.Size = new System.Drawing.Size(262, 30);
            this.textBox3.TabIndex = 40;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Cursor = System.Windows.Forms.Cursors.Default;
            this.label9.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label9.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label9.Location = new System.Drawing.Point(640, 24);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(88, 23);
            this.label9.TabIndex = 39;
            this.label9.Text = "Discount";
            // 
            // btn_update
            // 
            this.btn_update.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(249)))), ((int)(((byte)(221)))), ((int)(((byte)(143)))));
            this.btn_update.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_update.FlatAppearance.BorderSize = 2;
            this.btn_update.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(148)))), ((int)(((byte)(111)))), ((int)(((byte)(0)))));
            this.btn_update.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(246)))), ((int)(((byte)(219)))));
            this.btn_update.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_update.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_update.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(94)))), ((int)(((byte)(72)))), ((int)(((byte)(8)))));
            this.btn_update.Location = new System.Drawing.Point(358, 374);
            this.btn_update.Name = "btn_update";
            this.btn_update.Size = new System.Drawing.Size(126, 41);
            this.btn_update.TabIndex = 43;
            this.btn_update.Text = "Update";
            this.btn_update.UseVisualStyleBackColor = false;
            this.btn_update.Click += new System.EventHandler(this.btn_update_Click);
            // 
            // btn_print
            // 
            this.btn_print.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(187)))), ((int)(((byte)(222)))), ((int)(((byte)(249)))));
            this.btn_print.FlatAppearance.BorderColor = System.Drawing.Color.White;
            this.btn_print.FlatAppearance.BorderSize = 2;
            this.btn_print.FlatAppearance.MouseDownBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(31)))), ((int)(((byte)(110)))), ((int)(((byte)(173)))));
            this.btn_print.FlatAppearance.MouseOverBackColor = System.Drawing.Color.FromArgb(((int)(((byte)(117)))), ((int)(((byte)(173)))), ((int)(((byte)(217)))));
            this.btn_print.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
            this.btn_print.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btn_print.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(12)))), ((int)(((byte)(58)))), ((int)(((byte)(94)))));
            this.btn_print.Location = new System.Drawing.Point(506, 374);
            this.btn_print.Name = "btn_print";
            this.btn_print.Size = new System.Drawing.Size(113, 41);
            this.btn_print.TabIndex = 44;
            this.btn_print.Text = "Print Bill";
            this.btn_print.UseVisualStyleBackColor = false;
            this.btn_print.Click += new System.EventHandler(this.btn_print_Click);
            // 
            // txt_totalprice
            // 
            this.txt_totalprice.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_totalprice.Location = new System.Drawing.Point(20, 36);
            this.txt_totalprice.Name = "txt_totalprice";
            this.txt_totalprice.Size = new System.Drawing.Size(262, 30);
            this.txt_totalprice.TabIndex = 26;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Cursor = System.Windows.Forms.Cursors.Default;
            this.label10.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label10.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label10.Location = new System.Drawing.Point(17, 10);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(121, 23);
            this.label10.TabIndex = 25;
            this.label10.Text = "TOTAL PRICE";
            // 
            // panel5
            // 
            this.panel5.BackColor = System.Drawing.Color.Linen;
            this.panel5.Controls.Add(this.txt_totalprice);
            this.panel5.Controls.Add(this.label10);
            this.panel5.Location = new System.Drawing.Point(711, 292);
            this.panel5.Name = "panel5";
            this.panel5.Size = new System.Drawing.Size(301, 83);
            this.panel5.TabIndex = 45;
            // 
            // db_dataGridView1
            // 
            this.db_dataGridView1.AllowUserToDeleteRows = false;
            this.db_dataGridView1.AutoGenerateColumns = false;
            this.db_dataGridView1.ColumnHeadersHeightSizeMode = System.Windows.Forms.DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            this.db_dataGridView1.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.productNameDataGridViewTextBoxColumn,
            this.quantityDataGridViewTextBoxColumn,
            this.priceDataGridViewTextBoxColumn,
            this.descriptionDataGridViewTextBoxColumn,
            this.categoryDataGridViewTextBoxColumn,
            this.idDataGridViewTextBoxColumn});
            this.db_dataGridView1.DataSource = this.baseProductBindingSource;
            this.db_dataGridView1.Location = new System.Drawing.Point(64, 421);
            this.db_dataGridView1.Name = "db_dataGridView1";
            this.db_dataGridView1.ReadOnly = true;
            this.db_dataGridView1.RowHeadersWidth = 51;
            this.db_dataGridView1.RowTemplate.Height = 24;
            this.db_dataGridView1.Size = new System.Drawing.Size(982, 261);
            this.db_dataGridView1.TabIndex = 46;
            this.db_dataGridView1.CellContentClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.db_procardsDataGridView_CellClick);
            // 
            // txt_productname
            // 
            this.txt_productname.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_productname.Location = new System.Drawing.Point(330, 328);
            this.txt_productname.Name = "txt_productname";
            this.txt_productname.Size = new System.Drawing.Size(262, 30);
            this.txt_productname.TabIndex = 39;
            this.txt_productname.TextChanged += new System.EventHandler(this.txt_productId_TextChanged);
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label11.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(79)))), ((int)(((byte)(172)))), ((int)(((byte)(189)))));
            this.label11.Location = new System.Drawing.Point(328, 292);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(143, 23);
            this.label11.TabIndex = 37;
            this.label11.Text = "Product Name";
            // 
            // txt_customername
            // 
            this.txt_customername.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.txt_customername.Location = new System.Drawing.Point(20, 328);
            this.txt_customername.Name = "txt_customername";
            this.txt_customername.Size = new System.Drawing.Size(262, 30);
            this.txt_customername.TabIndex = 37;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Cursor = System.Windows.Forms.Cursors.Default;
            this.label13.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label13.ForeColor = System.Drawing.Color.FromArgb(((int)(((byte)(237)))), ((int)(((byte)(120)))), ((int)(((byte)(153)))));
            this.label13.Location = new System.Drawing.Point(16, 292);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(160, 23);
            this.label13.TabIndex = 39;
            this.label13.Text = "Customer Name";
            // 
            // productNameDataGridViewTextBoxColumn
            // 
            this.productNameDataGridViewTextBoxColumn.DataPropertyName = "ProductName";
            this.productNameDataGridViewTextBoxColumn.HeaderText = "ProductName";
            this.productNameDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.productNameDataGridViewTextBoxColumn.Name = "productNameDataGridViewTextBoxColumn";
            this.productNameDataGridViewTextBoxColumn.ReadOnly = true;
            this.productNameDataGridViewTextBoxColumn.Width = 125;
            // 
            // quantityDataGridViewTextBoxColumn
            // 
            this.quantityDataGridViewTextBoxColumn.DataPropertyName = "Quantity";
            this.quantityDataGridViewTextBoxColumn.HeaderText = "Quantity";
            this.quantityDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.quantityDataGridViewTextBoxColumn.Name = "quantityDataGridViewTextBoxColumn";
            this.quantityDataGridViewTextBoxColumn.ReadOnly = true;
            this.quantityDataGridViewTextBoxColumn.Width = 125;
            // 
            // priceDataGridViewTextBoxColumn
            // 
            this.priceDataGridViewTextBoxColumn.DataPropertyName = "Price";
            this.priceDataGridViewTextBoxColumn.HeaderText = "Price";
            this.priceDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.priceDataGridViewTextBoxColumn.Name = "priceDataGridViewTextBoxColumn";
            this.priceDataGridViewTextBoxColumn.ReadOnly = true;
            this.priceDataGridViewTextBoxColumn.Width = 125;
            // 
            // descriptionDataGridViewTextBoxColumn
            // 
            this.descriptionDataGridViewTextBoxColumn.DataPropertyName = "Description";
            this.descriptionDataGridViewTextBoxColumn.HeaderText = "Description";
            this.descriptionDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.descriptionDataGridViewTextBoxColumn.Name = "descriptionDataGridViewTextBoxColumn";
            this.descriptionDataGridViewTextBoxColumn.ReadOnly = true;
            this.descriptionDataGridViewTextBoxColumn.Width = 125;
            // 
            // categoryDataGridViewTextBoxColumn
            // 
            this.categoryDataGridViewTextBoxColumn.DataPropertyName = "Category";
            this.categoryDataGridViewTextBoxColumn.HeaderText = "Category";
            this.categoryDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.categoryDataGridViewTextBoxColumn.Name = "categoryDataGridViewTextBoxColumn";
            this.categoryDataGridViewTextBoxColumn.ReadOnly = true;
            this.categoryDataGridViewTextBoxColumn.Width = 125;
            // 
            // idDataGridViewTextBoxColumn
            // 
            this.idDataGridViewTextBoxColumn.DataPropertyName = "Id";
            this.idDataGridViewTextBoxColumn.HeaderText = "Id";
            this.idDataGridViewTextBoxColumn.MinimumWidth = 6;
            this.idDataGridViewTextBoxColumn.Name = "idDataGridViewTextBoxColumn";
            this.idDataGridViewTextBoxColumn.ReadOnly = true;
            this.idDataGridViewTextBoxColumn.Width = 125;
            // 
            // baseProductBindingSource
            // 
            this.baseProductBindingSource.DataSource = typeof(MarketManagement.BaseProduct);
            // 
            // Billing
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(11F, 22F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.label13);
            this.Controls.Add(this.txt_customername);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.txt_productname);
            this.Controls.Add(this.db_dataGridView1);
            this.Controls.Add(this.panel5);
            this.Controls.Add(this.btn_print);
            this.Controls.Add(this.btn_update);
            this.Controls.Add(this.textBox3);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.btn_removecart);
            this.Controls.Add(this.panel4);
            this.Controls.Add(this.panel3);
            this.Controls.Add(this.panel2);
            this.Controls.Add(this.btn_remove_selected);
            this.Controls.Add(this.btn_save);
            this.Controls.Add(this.btn_addtocard);
            this.Font = new System.Drawing.Font("Century Gothic", 11.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "Billing";
            this.Size = new System.Drawing.Size(1117, 696);
            this.Load += new System.EventHandler(this.UC_Billing_Load);
            this.panel2.ResumeLayout(false);
            this.panel2.PerformLayout();
            this.panel3.ResumeLayout(false);
            this.panel3.PerformLayout();
            this.panel4.ResumeLayout(false);
            this.panel4.PerformLayout();
            this.panel5.ResumeLayout(false);
            this.panel5.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.db_dataGridView1)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.baseProductBindingSource)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btn_remove_selected;
        private System.Windows.Forms.Button btn_save;
        private System.Windows.Forms.Button btn_addtocard;
        private System.Windows.Forms.TextBox txt_productprice;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txt_contact;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txt_productId;
        private System.Windows.Forms.TextBox txt_customerid;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.Label lbl_productid;
        private System.Windows.Forms.TextBox txt_productquantity;
        private System.Windows.Forms.TextBox txt_address;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Panel panel2;
        private System.Windows.Forms.Panel panel3;
        private System.Windows.Forms.Panel panel4;
        private System.Windows.Forms.Button btn_removecart;
        private System.Windows.Forms.TextBox textBox3;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Button btn_update;
        private System.Windows.Forms.Button btn_print;
        private System.Windows.Forms.TextBox txt_totalprice;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Panel panel5;
        private System.Windows.Forms.TextBox txt_invoiceno;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox dtp_invoicedate;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.DataGridView db_dataGridView1;
        private System.Windows.Forms.TextBox txt_productname;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label lbl_customerid;
        private System.Windows.Forms.TextBox txt_customername;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.DataGridViewTextBoxColumn productNameDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn quantityDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn priceDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn descriptionDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn categoryDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn idDataGridViewTextBoxColumn;
        private System.Windows.Forms.BindingSource baseProductBindingSource;
    }
}
